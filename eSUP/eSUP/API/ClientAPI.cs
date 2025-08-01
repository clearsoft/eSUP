﻿using eSUP.DTO;
using eSUP.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;
using System.Threading.Channels;

namespace eSUP.API;

public static class ClientAPI
{
    public static void MapClientAPI(this WebApplication app)
    {
        app.MapPost("/api/planner/new", async ([FromBody] PlannerDto dto, HttpContext context, MainContext dbContext) =>
        {
            var planner = Utilities.CreateNewSUP(dto);
            dbContext.Planners.Add(planner);
            await dbContext.SaveChangesAsync();
            return Results.Ok();
        });

        app.MapGet("/api/planner/summary/{plannerId}", async (Guid plannerId, MainContext dbContext) =>
        {
            var planner = await dbContext.Planners.Include(p => p.Exercises).ThenInclude(e => e.Questions).ThenInclude(q => q.Parts).ThenInclude(p => p.Users).FirstOrDefaultAsync(p => p.Id == plannerId);
            if (planner is null)
                return Results.BadRequest();

            // We fill this with all that is needed for the table display
            PlannerProgressDto progressPackage = new();

            // TODO:  Reverse the logic here - Loop through exercises then users.
            var users = dbContext.Users.Where(u => u.Planners.Any(p => p.Id == plannerId));
            await users.ForEachAsync(u =>
            {
                var studentExercise = new StudentProgressDto
                {
                    Id = new Guid(u.Id),
                    EMail = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName
                };
                planner.Exercises.OrderBy(e => e.Sequence).ToList().ForEach(e =>
                {
                    var partCount = e.Questions.SelectMany(q => q.Parts).Where(p => p.Users.Contains(u)).Count();
                    var exercise = new ExerciseDto()
                    {
                        Id = e.Id,
                        Title = e.Title ?? "-",
                        PartCount = partCount
                    };
                    studentExercise.Exercises.Add(exercise);
                });
                progressPackage.StudentProgresses.Add(studentExercise);
            });

            return Results.Ok(progressPackage);
        });

        app.MapGet("/api/planner/detail/{exerciseId}/{userId}", async (Guid exerciseId, Guid userId, MainContext dbContext) =>
        {
            var exercise = await dbContext.Exercises.Include(e => e.Questions).ThenInclude(q => q.Parts).ThenInclude(p => p.Users).FirstOrDefaultAsync(e => e.Id == exerciseId);
            if (exercise is null)
                return Results.BadRequest();
            var user = await dbContext.Users.FindAsync(userId.ToString());
            var questions = exercise.Questions.OrderBy(q => q.Sequence).Select(q => q.Map(user)).ToList();
            return Results.Ok(questions);

        });

        app.MapGet("/api/planner/summary-full/{plannerId}", async (Guid plannerId, MainContext dbContext) =>
        {
            var planner = await dbContext.Planners.Include(p => p.Exercises).ThenInclude(e => e.Questions).ThenInclude(q => q.Parts).ThenInclude(p => p.Users).FirstOrDefaultAsync(p => p.Id == plannerId);
            if (planner is null)
                return Results.BadRequest();

            // We fill this with all that is needed for the table display
            PlannerProgressDto progressPackage = new()
            {
                Title = planner.Title ?? "-"
            };

            // Top heading = exercises
            HeaderDto exerciseheader = new();
            var exercises = planner.Exercises.OrderBy(e => e.Sequence).ToList();
            exercises.ForEach(e =>
            {
                exerciseheader.Span = e.Questions.Count;

                // This puts the exercise title in the first box and fills the rest with empty strings
                exerciseheader.Headings.Add(e.Title! ?? "-");
                for (int i = 0; i < exerciseheader.Span - 1; i++)
                    exerciseheader.Headings.Add("");
            });
            progressPackage.HeadingItems.Add(exerciseheader);

            // Second heading = questions
            // TODO:  These can be combined
            HeaderDto questionHeader = new();
            exercises.ForEach(e =>
            {
                e.Questions.OrderBy(q => q.Sequence).ToList().ForEach(q => questionHeader.Headings.Add(q.Title! ?? "-"));
            });
            progressPackage.HeadingItems.Add(questionHeader);

            var plannerWithUsers = await dbContext.Planners.Include(p => p.Users).FirstOrDefaultAsync(p => p.Id == plannerId);
            List<StudentProgressDto> studentProgressList = [];
            plannerWithUsers!.Users.ForEach(user =>
            {
                StudentProgressDto output = new()
                {
                    FirstName = user is null ? "-" : user.FirstName,
                    LastName = user is null ? "-" : user.LastName
                };
                exercises.ForEach(e =>
                {
                    e.Questions.OrderBy(q => q.Sequence).ToList().ForEach(q =>
                    {
                        int count = q.Parts.Count(p => p.Users.Contains(user!));
                        output.PartsCompleteCount.Add(count);
                    });
                });
                studentProgressList.Add(output);
            });
            progressPackage.StudentProgresses = studentProgressList;
            return Results.Ok(progressPackage);
        });

        app.MapPost("/api/planner/save", async (ClaimsPrincipal principal, [FromBody] PlannerDto dto, MainContext dbContext) =>
        {
            try
            {
                var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await dbContext.Users.Include(u => u.Parts).FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                    return Results.NotFound("User not found");

                var partIdList = dto.Exercises.SelectMany(e => e.Questions).SelectMany(q => q.Parts).Where(p => p.IsCompleted).Select(p => p.Id).ToList();

                user.Parts.Clear();
                await dbContext.SaveChangesAsync();

                var selectedParts = await dbContext.Parts.Include(p => p.Users).Where(p => partIdList.Contains(p.Id)).ToListAsync();

                user.Parts.AddRange(selectedParts);

                await dbContext.SaveChangesAsync();
                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.InternalServerError(ex.Message);
            }
        });

        app.MapPost("/api/planner/parts", async ([FromBody] Stack<PartDto> changes, MainContext dbContext) =>
        {
            while (changes.Count > 0)
            {
                var change = changes.Pop();
                var part = dbContext.Parts.Find(change.Id);
                if (part != null)
                {
                    part.IsLevelAt = change.IsLevelAt;
                    part.IsLevelAbove = change.IsLevelAbove;
                    part.IsLevelBelow = change.IsLevelBelow;
                }
            }
            await dbContext.SaveChangesAsync();
            return Results.Ok();
        });

        app.MapDelete("/api/planner/parts", async ([FromBody] Stack<PartDto> deletions, MainContext dbContext) =>
        {
            while(deletions.Count > 0)
            {
                var d = deletions.Pop();
                var p = dbContext.Parts.Find(d.Id);
                if (p is not null)
                    dbContext.Parts.Remove(p);
            }
            await dbContext.SaveChangesAsync();
            return Results.Ok();
        });

        app.MapPut("/api/planner/parts", async ([FromBody] Stack<PartDto> modifications, MainContext dbContext) =>
        {
            while (modifications.Count > 0)
            {
                var m = modifications.Pop();
                var p = dbContext.Parts.Find(m.Id);
                if (p is not null)
                {
                    p.IsLevelAbove = m.IsLevelAbove;
                    p.IsLevelAt = m.IsLevelAt;
                    p.IsLevelBelow = m.IsLevelBelow;
                }
            }
            await dbContext.SaveChangesAsync();
            return Results.Ok();
        });

        app.MapDelete("/api/planner/questions", async ([FromBody] Stack<QuestionDto> deletions, MainContext dbContext) =>
        {
            while (deletions.Count > 0)
            {
                var d = deletions.Pop();
                var q = dbContext.Questions.Find(d.Id);
                if (q is not null)
                    dbContext.Questions.Remove(q);
            }
            await dbContext.SaveChangesAsync();
            return Results.Ok();
        });

        app.MapPost("/api/planner/rename/{id}", async ([FromBody] string title, Guid id, MainContext dbContext) =>
        {
            var planner = await dbContext.Planners.FirstOrDefaultAsync(p => p.Id == id);
            if (planner == null)
                return Results.NotFound("Planner not found");

            planner.Title = title;
            await dbContext.SaveChangesAsync();

            return Results.Ok();
        });

        app.MapGet("/api/planner/{id}", async (ClaimsPrincipal principal, string id, MainContext dbContext) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null)
                return Results.NotFound("User not found");

            var planner = await dbContext.Planners.Include(p => p.Exercises).ThenInclude(e => e.Questions).ThenInclude(q => q.Parts).ThenInclude(p => p.Users).FirstOrDefaultAsync(p => p.Id == new Guid(id));
            if (planner == null)
                return Results.NotFound("Planner not found");

            PlannerDto dto = Utilities.CompilePlannerDto(planner, user);

            return Results.Ok(dto);
        });

        // This endpoint is used to get the full list of users marked with those for this specific planner
        app.MapGet("/api/planner/assign/{id}", async (Guid id, MainContext dbContext, UserManager<ApplicationUser> userManager) =>
        {
            var planner = await dbContext.Planners.FindAsync(id);
            if (planner is null)
                return Results.BadRequest();

            List<UserInformationDto> userList = [];
            // Get all users and add role and assignment status
            await dbContext.Users.Include(u => u.Planners).ForEachAsync(u =>
                    {
                        UserInformationDto userDto = u.Map();
                        userDto.Role = userManager.GetRolesAsync(u).Result.FirstOrDefault();
                        userDto.IsAssigned = u.Planners.Contains(planner);
                        userList.Add(userDto);
                    });

            var assignmentDto = new AssignmentDto()
            {
                Id = planner.Id,
                Title = planner.Title,
                Users = userList
            };
            return Results.Ok(assignmentDto);
        });

        // This endpoint is used to assign, reassign or deassign users to a planner
        app.MapPost("/api/planner/assign/{id}", async (Guid id, AssignmentDto dto, MainContext dbContext) =>
        {
            try
            {
                var planner = await dbContext.Planners.Include(p => p.Users).Where(p => p.Id == id).FirstOrDefaultAsync();
                if (planner is null)
                    return Results.BadRequest("Planner not found");

                planner.Users.Clear();

                var selectedUserIds = dto.Users.Where(u => u.IsAssigned).Select(u => u.UserId.ToString()).ToList();
                // Fetch actual user entities in one query
                var selectedUsers = await dbContext.Users
                    .Where(u => selectedUserIds.Contains(u.Id))
                    .ToListAsync();

                // Add to planner
                planner.Users.AddRange(selectedUsers);

                await dbContext.SaveChangesAsync();
                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        });

        app.MapDelete("api/planner/{id}", async (string id, MainContext dbContext) =>
        {
            var planner = await dbContext.Planners.FindAsync(new Guid(id));
            if (planner == null)
                return Results.NotFound();
            dbContext.Planners.Remove(planner);
            dbContext.SaveChanges();
            return Results.Ok();
        });

        app.MapGet("/api/planners", async (ClaimsPrincipal principal, MainContext dbContext, UserManager<ApplicationUser> userManager) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null)
                return Results.NotFound("User not found");

            List<PlannerDto> planners;
            var roles = userManager.GetRolesAsync(user).Result;
            if (roles.Contains("Admin") || roles.Contains("Teacher"))
                planners = await dbContext.Planners.Select(p => p.Map()).ToListAsync();
            else
                planners = await dbContext.Planners.Include(p => p.Users).Where(p => p.Users.Contains(user)).Select(p => p.Map()).ToListAsync();

            return Results.Ok(planners);
        });

        app.MapGet("/api/users", async (MainContext dbContext, UserManager<ApplicationUser> userManager) =>
        {
            List<UserInformationDto> userList = [];
            // Get all users and add role and assignment status
            await dbContext.Users.ForEachAsync(u =>
            {
                UserInformationDto userDto = u.Map();
                userDto.Role = userManager.GetRolesAsync(u).Result.FirstOrDefault();
                userList.Add(userDto);
            });
            return Results.Ok(userList);
        });

        app.MapPost("/api/users/upload", (List<UserInformationDto> newUsers, MainContext dbContext, UserManager<ApplicationUser> userManager) =>
            {
                newUsers.ForEach(u =>
                {
                    var user = new ApplicationUser
                    {
                        UserName = u.Email,
                        Email = u.Email,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Group = u.Group,
                        EmailConfirmed = true
                    };
                    var result = userManager.CreateAsync(user, u.Password ?? "Password1!").Result;
                    if (result.Succeeded)
                        userManager.AddToRoleAsync(user, "Student").Wait();
                });
                return Results.Ok("Users uploaded successfully.");
            })
            .DisableAntiforgery();

        app.MapGet("/api/users/upgrade/{id}", async (Guid id, MainContext dbContext, UserManager<ApplicationUser> userManager) =>
        {
            var user = await userManager.FindByIdAsync(id.ToString());
            if (user is null)
                return Results.NotFound("User not found");

            // Upgrade
            await userManager.AddToRoleAsync(user, "Teacher");

            // Map into info object and add in role ... roles?
            UserInformationDto userInfo = user.Map();
            userInfo.Role = userManager.GetRolesAsync(user).Result.FirstOrDefault();
            return Results.Ok(userInfo);
        });

        app.MapGet("/api/users/downgrade/{id}", async (Guid id, MainContext dbContext, UserManager<ApplicationUser> userManager) =>
        {
            var user = await userManager.FindByIdAsync(id.ToString());
            if (user is null)
                return Results.NotFound("User not found");

            // Upgrade
            await userManager.RemoveFromRoleAsync(user, "Teacher");

            // Map into info object and add in role ... roles?
            UserInformationDto userInfo = user.Map();
            userInfo.Role = userManager.GetRolesAsync(user).Result.FirstOrDefault();
            return Results.Ok(userInfo);
        });

        app.MapPost("/api/users/delete", async ([FromBody] List<string> userIds, MainContext dbContext) =>
        {
            if (userIds == null || userIds.Count == 0)
                return Results.BadRequest("No user IDs provided");

            var usersToDelete = await dbContext.Users
                .Where(u => userIds.Contains(u.Id))
                .ToListAsync();

            if (usersToDelete.Count == 0)
                return Results.BadRequest("No users found");

            dbContext.Users.RemoveRange(usersToDelete);
            await dbContext.SaveChangesAsync();

            return Results.Ok();
        });
    }
}

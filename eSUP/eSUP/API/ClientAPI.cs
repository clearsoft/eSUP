using eSUP.Data;
using eSUP.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;

namespace eSUP.API;

public static class ClientAPI
{
    public static void MapClientAPI(this WebApplication app)
    {
        app.MapPost("/api/planner/new", ([FromBody] PlannerSpecificationDto specification, HttpContext context, MainContext dbContext) =>
        {
            if (specification == null)
            {
                return Results.BadRequest("Specification is null");
            }
            var SUP = Utilities.GenerateNewSUP(specification);
            return Results.Ok(SUP);
        });

        app.MapGet("/api/planner/summary/{plannerId}", async (Guid plannerId, HttpContext context, MainContext dbContext) =>
        {
            var planner = await dbContext.Planners.Include(p => p.Exercises).ThenInclude(e => e.Questions).ThenInclude(q => q.Parts).ThenInclude(p => p.Users).FirstOrDefaultAsync(p => p.Id == plannerId);
            if (planner is null)
                return Results.BadRequest();

            // We fill this with all that is needed for the table display
            PlannerProgressDto progressPackage = new()
            {
                Title = planner.Title
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
                    Name = user is null ? "-" : user.Email
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

        app.MapPost("/api/planner/update", async ([FromBody] List<PartDto> changes, HttpContext context, MainContext dbContext) =>
        {
            changes.ForEach(change =>
            {
                var part = dbContext.Parts.Find(change.Id);
                if (part != null)
                {
                    part.IsLevelAt = change.IsLevelAt;
                    part.IsLevelAbove = change.IsLevelAbove;
                    part.IsLevelBelow = change.IsLevelBelow;
                }
            });
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
        app.MapPost("/api/planner/assign/{id}", async (ClaimsPrincipal principal, Guid id, AssignmentDto dto, MainContext dbContext) =>
        {
            try
            {
                var planner = await dbContext.Planners.Include(p => p.Users).Where(p => p.Id == id).FirstOrDefaultAsync();
                if (planner is null)
                    return Results.BadRequest("Planner not found");

                var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await dbContext.Users.FindAsync(userId);
                if (user == null)
                    return Results.NotFound("User not found");

                planner.Users.Clear();
                user.Planners.Clear();

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

        app.MapGet("/api/planners", async (MainContext dbContext) =>
        {
            var planners = await dbContext.Planners.Select(p => p.Map()).ToListAsync();
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

        app.MapGet("/api/users/upgrade/{id}", async (Guid id, MainContext dbContext, UserManager<ApplicationUser> userManager) =>
        {
            var user = await userManager.FindByIdAsync(id.ToString());
            if (user is null)
                return Results.NotFound("User not found");

            // Upgrade
            await userManager.AddToRoleAsync(user, "Teacher");

            // Map into info ocject and add in role ... roles?
            UserInformationDto userInfo = user.Map();
            userInfo.Role = userManager.GetRolesAsync(user).Result.FirstOrDefault();
            return Results.Ok(userInfo);
        });


    }
}

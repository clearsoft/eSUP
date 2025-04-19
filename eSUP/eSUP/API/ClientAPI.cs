using eSUP.Data;
using eSUP.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Numerics;

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

        app.MapPost("/api/planner/save", async ([FromBody] PlannerDto dto, HttpContext context, MainContext dbContext) =>
        {
            var planner = Utilities.CreateNewSUP(dto);
            dbContext.Planners.Add(planner);
            await dbContext.SaveChangesAsync();
            return Results.Ok();
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

        app.MapGet("/api/planner/{id}", async (string id, MainContext dbContext) =>
        {
            var planner = await dbContext.Planners.Include(p => p.Exercises).ThenInclude(e => e.Questions).ThenInclude(q => q.Parts).FirstOrDefaultAsync(p => p.Id == new Guid(id));
            if (planner == null)
                return Results.NotFound();

            return Results.Ok(planner.Map());
        });

        // This endpoint is used to get the full list of users marked with those for this specific planner
        app.MapGet("/api/planner/assign/{id}", async (Guid id, MainContext dbContext, UserManager<ApplicationUser> userManager) =>
        {
            var planner = await dbContext.Planners.FindAsync(id);
            if (planner is null)
                return Results.BadRequest();

            List<UserInformationDto> userList = new();
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
                selectedUsers.ForEach(user => planner.Users.AddRange(selectedUsers));

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
            List<UserInformationDto> userList = new();
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

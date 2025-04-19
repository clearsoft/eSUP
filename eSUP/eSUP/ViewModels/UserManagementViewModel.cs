using eSUP.Data;
using eSUP.DTO;
using Microsoft.AspNetCore.Identity;

namespace eSUP.Client.ViewModels;

public class UserManagementViewModel(UserManager<ApplicationUser> _userManager, RoleManager<IdentityRole> _roleManager)
{
    private readonly UserManager<ApplicationUser> userManager = _userManager;
    //private readonly RoleManager<IdentityRole> roleManager = _roleManager;

    public List<UserInformationDto> Users { get; set; } = [];


    public async Task LoadUsersAsync()
    {
        var users = userManager.Users.ToList();

        Users = new List<UserInformationDto>();

        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            Users.Add(new UserInformationDto
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = roles.FirstOrDefault()
            });
        }
    }

    public async Task UpgradeRoleAsync(UserInformationDto dto)
    {
        var user = await userManager.FindByIdAsync(dto.UserId);
        var currentRoles = await userManager.GetRolesAsync(user);

        if (currentRoles.Contains("User"))
        {
            await userManager.RemoveFromRoleAsync(user, "User");
            await userManager.AddToRoleAsync(user, "Admin");
        }
        else if (currentRoles.Contains("Admin"))
        {
            await userManager.RemoveFromRoleAsync(user, "Admin");
            await userManager.AddToRoleAsync(user, "SuperAdmin");
        }

        await LoadUsersAsync();
    }
}


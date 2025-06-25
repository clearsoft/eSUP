using eSUP.Client.ViewModels;
using eSUP.DTO;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace eSUP.Client.Pages;

public partial class UserManagement(UserManagementViewModel _vm) : ComponentBase
{
    MudDataGrid<UserInformationDto>? userGrid;
    public HashSet<UserInformationDto>? SelectedUsers;

    public UserManagementViewModel vm { get; set; } = _vm;

    protected async Task UpgradeRole(UserInformationDto dto) => await vm!.UpgradeRoleAsync(dto);

    private async Task UploadUserListAsync(IBrowserFile file)
    {
        await vm.UploadUserListAsync(file);
        await userGrid!.ReloadServerData();
    }

    protected async void DeletedSelectedUsers()
    {
        var users = SelectedUsers!.Where(u => u.Role != "Admin").ToList();
        if (await vm.DeleteUsersAsync(users))
        {
            users.ForEach(user => vm.Users.Remove(user));
            await userGrid!.ReloadServerData();
        }
    }
}

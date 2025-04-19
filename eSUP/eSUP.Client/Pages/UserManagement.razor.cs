using eSUP.Client.ViewModels;
using eSUP.DTO;
using Microsoft.AspNetCore.Components;

namespace eSUP.Client.Pages;

public partial class UserManagement(UserManagementViewModel _vm) : ComponentBase
{
    public UserManagementViewModel vm { get; set; } = _vm;

    protected override async Task OnInitializedAsync()
    {
        await vm!.LoadUsersAsync();
    }

    protected async Task UpgradeRole(UserInformationDto dto)
    {
        await vm!.UpgradeRoleAsync(dto);
    }
}

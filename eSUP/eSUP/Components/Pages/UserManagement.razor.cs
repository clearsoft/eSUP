namespace eSUP.Components.Pages;

using eSUP.ViewModels;
using Microsoft.AspNetCore.Components;

public partial class UserManagement(UserManagementViewModel _vm) : ComponentBase
{
    public UserManagementViewModel vm { get; set; } = _vm;

    protected override async Task OnInitializedAsync()
    {
        await vm!.LoadUsersAsync();
    }
}

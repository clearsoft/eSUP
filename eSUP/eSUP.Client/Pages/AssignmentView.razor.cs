using eSUP.Client.ViewModels;
using eSUP.DTO;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.IO.Compression;
using System.Net.Http;

namespace eSUP.Client.Pages;

public partial class AssignmentView(AssignmentViewModel _vm, NavigationManager _navigationManager)
{
    private readonly AssignmentViewModel vm = _vm;
    private readonly NavigationManager navigationManager = _navigationManager;
    [Parameter]
    public string? PlannerId { get; set; }

    MudDataGrid<UserInformationDto>? userGrid;

    protected async override Task OnParametersSetAsync()
    {
        vm.PlannerId = PlannerId;
        await userGrid!.ReloadServerData();
    }

    private async Task SaveAssignmentAsync()
    {
        await vm.SaveAssignmentAsync();
        await Task.Delay(500);
        navigationManager.NavigateTo("/planners");
    }

    protected void ReturnToPlannerPage()
    {
        navigationManager.NavigateTo("planners");
    }
}

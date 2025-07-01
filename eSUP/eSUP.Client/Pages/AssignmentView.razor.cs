using eSUP.Client.ViewModels;
using eSUP.DTO;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.IO.Compression;
using System.Net.Http;
using static MudBlazor.CategoryTypes;

namespace eSUP.Client.Pages;

public partial class AssignmentView(AssignmentViewModel _vm, NavigationManager _navigationManager)
{
    private readonly AssignmentViewModel vm = _vm;
    private readonly NavigationManager navigationManager = _navigationManager;
    [Parameter]
    public string? PlannerId { get; set; }
    private string? searchString = null;

    MudDataGrid<UserInformationDto>? userGrid;

    protected async override Task OnParametersSetAsync()
    {
        vm.PlannerId = PlannerId;
        await vm.LoadAssignmentAsync(PlannerId);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await userGrid!.ReloadServerData();
        await base.OnAfterRenderAsync(firstRender);
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
    private Task OnSearch(string text)
    {
        vm.SearchString = text;
        return userGrid!.ReloadServerData();
    }
}

using eSUP.Client.ViewModels;
using Microsoft.AspNetCore.Components;

namespace eSUP.Client.Pages;

public partial class AssignmentView(AssignmentViewModel _vm, NavigationManager _navigationManager)
{
    private readonly AssignmentViewModel vm = _vm;
    private NavigationManager navigationManager = _navigationManager;

    [Parameter]
    public string? PlannerId { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        await vm.LoadAssignmentAsync(PlannerId);
        await base.OnParametersSetAsync();
        StateHasChanged();
    }

    private async Task SaveAssignmentAsync()
    {
        await vm.SaveAssignmentAsync(PlannerId);
        await Task.Delay(500);
        navigationManager.NavigateTo("/planners");
    }
}

using eSUP.Client.ViewModels;
using eSUP.DTO;
using Microsoft.AspNetCore.Components;

namespace eSUP.Client.Pages;

public partial class EditorView(CreatorViewModel _vm, NavigationManager _navigationManager)
{
    private readonly CreatorViewModel vm = _vm;
    private readonly NavigationManager navigationManager = _navigationManager;
    private readonly Stack<PartDto> changeStack = new();

    [Parameter]
    public string? PlannerId { get; set; }

    protected override async Task OnParametersSetAsync() // Marked as async to fix CS4032
    {
        await vm.LoadPlannerAsync(PlannerId);
        await base.OnParametersSetAsync(); // Ensure base method is awaited
        StateHasChanged();
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
    }

    public void RefreshUI()
    {
        StateHasChanged();
    }

    private void RecordChange(PartDto part)
    {
        changeStack.Push(part);
    }

    private async Task UpdatePlanner()
    {
        await vm.UpdatePlannerAsync(changeStack);
        await Task.Delay(200);
        navigationManager.NavigateTo("planners");
    }

    protected void ReturnToPlannerPage()
    {
        navigationManager.NavigateTo("planners");
    }
}
    
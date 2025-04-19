using eSUP.Client.ViewModels;
using eSUP.DTO;
using Microsoft.AspNetCore.Components;

namespace eSUP.Client.Pages;

public partial class StudentView(StudentViewModel _vm, NavigationManager _navigationManager)
{
    private readonly StudentViewModel vm = _vm;
    private NavigationManager navigationManager = _navigationManager;
    private Stack<PartDto> changeStack = new();

    [Parameter]
    public string? PlannerId { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        await vm.LoadAssignmentAsync(PlannerId);
        await base.OnParametersSetAsync();
    }

    public void RefreshUI()
    {
        StateHasChanged();
    }
    private void RecordChange(PartDto part)
    {
        changeStack.Push(part);
    }

    private async Task SaveAssignmentAsync()
    {
        await vm.SaveAssignmentAsync(PlannerId);
        Task.Delay(500).Wait();
        navigationManager.NavigateTo("planners");
    }
}

using eSUP.Client.ViewModels;
using eSUP.DTO;
using Microsoft.AspNetCore.Components;

namespace eSUP.Client.Pages;

public partial class StudentView(StudentViewModel _vm, NavigationManager _navigationManager)
{
    private readonly StudentViewModel vm = _vm;
    private NavigationManager navigationManager = _navigationManager;
    private Queue<PartDto> changeStack = new();

    [Parameter]
    public string? PlannerId { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        await vm.LoadProgressAsync(PlannerId);
        await base.OnParametersSetAsync();
    }

    public void RefreshUI()
    {
        StateHasChanged();
    }
    private void RecordChange(PartDto part)
    {
        changeStack.Enqueue(part);
    }

    private async Task SaveProgressAsync()
    {
        await vm.SaveProgressAsync();
        await Task.Delay(500);
        navigationManager.NavigateTo("planners");
    }
}

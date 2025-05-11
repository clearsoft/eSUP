using eSUP.Client.ViewModels;
using Microsoft.AspNetCore.Components;

namespace eSUP.Client.Pages;

public partial class ProgressViewFull(ProgressViewModelFull _vm, NavigationManager _nav)
{
    private readonly ProgressViewModelFull vm = _vm;
    private readonly NavigationManager navigationManager = _nav;

    [Parameter]
    public string? PlannerId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await vm.LoadSummaryAsync(PlannerId!);
        await base.OnInitializedAsync();
    }

    protected void ReturnToPlannerPage()
    {
        navigationManager.NavigateTo("planners");
    }
}

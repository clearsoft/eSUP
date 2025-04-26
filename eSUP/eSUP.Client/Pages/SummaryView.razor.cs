using eSUP.Client.ViewModels;
using Microsoft.AspNetCore.Components;

namespace eSUP.Client.Pages;

public partial class SummaryView(SummaryViewModel _vm, NavigationManager _nav)
{
    private readonly SummaryViewModel vm = _vm;
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

    // Dispose logic if needed
    public void Dispose()
    {
    }
}

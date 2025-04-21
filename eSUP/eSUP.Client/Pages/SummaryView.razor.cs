using eSUP.Client.ViewModels;
using Microsoft.AspNetCore.Components;

namespace eSUP.Client.Pages;

public partial class SummaryView(SummaryViewModel _vm)
{
    public SummaryViewModel vm = _vm;

    [Parameter]
    public string? PlannerId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await vm.LoadSummaryAsync(PlannerId!);
        await base.OnInitializedAsync();
    }
    // Dispose logic if needed
    public void Dispose()
    {
    }
}

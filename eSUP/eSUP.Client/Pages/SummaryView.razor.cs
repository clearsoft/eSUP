using eSUP.Client.ViewModels;

namespace eSUP.Client.Pages;

public partial class SummaryView(SummaryViewModel _vm)
{
    public SummaryViewModel ViewModel = _vm;
    // Dispose logic if needed
    public void Dispose()
    {
    }
}

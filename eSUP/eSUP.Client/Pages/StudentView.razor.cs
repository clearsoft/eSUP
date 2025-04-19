using eSUP.Client.ViewModels;

namespace eSUP.Client.Pages;

public partial class StudentView(StudentViewModel _vm)
{
    public StudentViewModel ViewModel = _vm;
    public void Dispose()
    {
        // Dispose logic if needed
    }
}

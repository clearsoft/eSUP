using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace eSUP.Client.Components;

public partial class ConfirmationDialog()
{

    [CascadingParameter]
    public IMudDialogInstance dialog { get; set; } = default!;

    private void OK() => dialog?.Close(DialogResult.Ok(""));

    private void Cancel() => dialog?.Cancel();
}

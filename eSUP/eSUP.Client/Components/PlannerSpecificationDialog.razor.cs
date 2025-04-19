using eSUP.DTO;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace eSUP.Client.Components;

public partial class PlannerSpecificationDialog()
{
    public PlannerSpecificationDto dto { get; set; } = new();

    [CascadingParameter]
    public IMudDialogInstance dialog { get; set; } = default!;

    private void OK()=> dialog?.Close(DialogResult.Ok(dto));
   
    private void Cancel() => dialog?.Cancel();
}

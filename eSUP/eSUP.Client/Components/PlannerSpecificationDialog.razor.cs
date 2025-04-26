using eSUP.DTO;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace eSUP.Client.Components;

public partial class PlannerSpecificationDialog()
{
    public PlannerSpecificationDto dto { get; set; } = new();
    private PlannerDto? planner { get; set; }
    public bool IsFromFile { get; set; } = false;

    [CascadingParameter]
    public IMudDialogInstance dialog { get; set; } = default!;

    private async Task UploadFilesAsync(IBrowserFile file)
    {
        // File processing is local into the specification
        var fileStream = file.OpenReadStream(10 * 1024 * 1024); // Limit to 10 MB
        var txt = await new StreamReader(fileStream).ReadToEndAsync();
        planner = Utilities.GenerateSUPFromTextDefinition(file.Name, txt);
        IsFromFile = true;
    }

    private void OK()
    {
        if (!IsFromFile)
            planner = Utilities.GenerateNewSUP(dto);

        planner!.Exercises.ForEach(e => e.LevelSelected = "0");
        dialog?.Close(DialogResult.Ok(planner));
    }

    private void Cancel() => dialog?.Cancel();
}

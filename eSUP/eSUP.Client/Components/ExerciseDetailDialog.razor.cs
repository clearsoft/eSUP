using eSUP.DTO;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace eSUP.Client.Components;

public partial class ExerciseDetailDialog()
{
    [Parameter]
    public string? Title { get; set; }
    [Parameter]
    public List<QuestionDto> Questions { get; set; } = [];
    [Parameter]
    public string? StudentName { get; set; }

    [CascadingParameter]
    public IMudDialogInstance dialog { get; set; } = default!;

    private void OK() => dialog?.Close();
}

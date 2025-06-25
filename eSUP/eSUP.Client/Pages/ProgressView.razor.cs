using eSUP.Client.Components;
using eSUP.Client.ViewModels;
using eSUP.DTO;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Numerics;

namespace eSUP.Client.Pages;

public partial class ProgressView(ProgressViewModel _vm, NavigationManager _nav, IDialogService _dialogService)
{
    private readonly ProgressViewModel vm = _vm;
    private readonly NavigationManager navigationManager = _nav;
    private readonly IDialogService dialogService = _dialogService;

    [Parameter]
    public string? PlannerId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await vm.LoadSummaryAsync(PlannerId!);
        await base.OnInitializedAsync();
    }

    protected async void ExpandResult(ExerciseDto exercise, StudentProgressDto student)
    {
        var questions = await vm.GetDetailAsync(exercise.Id, student.Id);
        var options = new DialogOptions { CloseOnEscapeKey = true };
        var dialogParameters = new DialogParameters<ExerciseDetailDialog>
        {
            { p => p.Title, exercise.Title},
            { p => p.Questions, questions },
            { p => p.StudentName, student.FullName}
        };

        // Show details - this doesn't require any interaction by the operator, so no action is needed on close
        var dialog = await dialogService.ShowAsync<ExerciseDetailDialog>("Exercise Detail", dialogParameters, options);
        var result = await dialog.Result;
    }

    protected void ReturnToPlannerPage() => navigationManager.NavigateTo("planners");
}

using Microsoft.AspNetCore.Components;
using eSUP.Client.Components;
using eSUP.Client.ViewModels;
using eSUP.DTO;
using MudBlazor;
using System.Net.Http;

namespace eSUP.Client.Pages;

public partial class CreatorView(CreatorViewModel _vm, NavigationManager _navigationManager)
{
    private readonly CreatorViewModel vm = _vm;
    private NavigationManager navigationManager = _navigationManager;

    protected override Task OnInitializedAsync()
    {
        vm.Planner = new PlannerDto();
        return base.OnInitializedAsync();
    }
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await ShowPlannerSpecificationDialogAsync();
        }
    }

    public void RefreshUI()
    {
        StateHasChanged();
    }

    private async Task ShowPlannerSpecificationDialogAsync()
    {
        var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true, MaxWidth = MaxWidth.Small };
        var dialog = await dialogService.ShowAsync<PlannerSpecificationDialog>("Create New Student Unit Planner", options);
        var result = await dialog.Result;
        if (result is not null && result.Data is not null && !result.Canceled)
        {
            var plannerSpecification = result.Data as PlannerSpecificationDto;
            if (plannerSpecification is not null)
            {
                vm.CreateNewPlannerAsync(plannerSpecification);
                StateHasChanged();
            }
        }
    }

    private async Task SavePlanner()
    {
        await vm.SavePlannerAsync();
        navigationManager.NavigateTo("planners");
    }
}

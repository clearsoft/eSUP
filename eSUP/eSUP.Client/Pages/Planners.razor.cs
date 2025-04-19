using eSUP.Client.Components;
using eSUP.Client.ViewModels;
using eSUP.DTO;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace eSUP.Client.Pages
{
    public partial class Planners(NavigationManager _navigationManager, PlannerViewModel _vm, IDialogService _dialogService, AuthenticationStateProvider _authenticationStateProvider)
    {
        private readonly NavigationManager navigationManager = _navigationManager;
        private readonly PlannerViewModel vm = _vm;
        private readonly IDialogService dialogService = _dialogService;
        private readonly AuthenticationStateProvider authenticationStateProvider = _authenticationStateProvider;

        private bool canManagePlanners = false;

        protected override async Task OnInitializedAsync()
        {
            var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            canManagePlanners = user.IsInRole("Admin") || user.IsInRole("Teacher");
            await base.OnInitializedAsync();
        }
        protected override async Task OnParametersSetAsync()
        {
            await vm.LoadAvailablePlannersAsync();
            await base.OnParametersSetAsync();
        }

        internal void OpenPlanner(PlannerDto planner)
        {

        }

        internal void AssignPlanner(PlannerDto planner)
        {
            navigationManager.NavigateTo($"assign-planner/{planner.Id}");
        }

        internal void EditPlanner(PlannerDto planner)
        {
            navigationManager.NavigateTo($"edit-planner/{planner.Id}");
        }

        public async void DeletePlanner(PlannerDto planner)
        {
            // Show confirmation dialog

            var options = new DialogOptions { CloseOnEscapeKey = true };
            var dialog = await dialogService.ShowAsync<ConfirmationDialog>("Delete Planner", options);
            var result = await dialog.Result;
            if (result is not null && result.Data is not null && !result.Canceled)
            {
                // Call the delete method in the ViewModel
                await vm.DeletePlannerAsync(planner.Id);
                StateHasChanged();
            }
        }

        public void GoToCreate()
        {
            navigationManager.NavigateTo("/create-planner");
        }
    }
}


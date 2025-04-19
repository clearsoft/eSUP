using eSUP.DTO;
using System.Collections.ObjectModel;
using System.Net.Http.Json;


namespace eSUP.Client.ViewModels
{
    public class PlannerViewModel(HttpClient _httpClient)
    {
        private readonly HttpClient httpClient = _httpClient;

        public ObservableCollection<PlannerDto>? AvailablePlanners { get; set; } = [];

        internal async Task LoadAvailablePlannersAsync()
        {
            var response = await httpClient.GetFromJsonAsync<List<PlannerDto>>("api/planners");
            if (response is not null)
                AvailablePlanners = new ObservableCollection<PlannerDto>(response);
        }

        internal async Task DeletePlannerAsync(Guid? id)
        {
            var response = await httpClient.DeleteAsync($"api/planner/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                var planner = AvailablePlanners?.FirstOrDefault(p => p.Id == id);
                if(planner is not null)
                    AvailablePlanners?.Remove(planner);
            }
        }

        internal async Task CreateNewPlannerAsync(PlannerDto planner)
        {
            var response = await httpClient.PostAsJsonAsync("api/planner", planner);
            if (response.IsSuccessStatusCode)
            {
                var newPlanner = await response.Content.ReadFromJsonAsync<PlannerDto>();
                if (newPlanner is not null)
                {
                    AvailablePlanners?.Add(newPlanner);
                }
            }
        }
        internal async Task<PlannerDto?> GetPlannerByIdAsync(Guid id)
        {
            var response = await httpClient.GetAsync($"api/planner/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<PlannerDto>();
            }
            return null;
        }
    }
}

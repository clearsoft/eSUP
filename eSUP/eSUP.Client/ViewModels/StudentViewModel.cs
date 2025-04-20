using eSUP.DTO;
using System.Net.Http.Json;

namespace eSUP.Client.ViewModels;

public class StudentViewModel(HttpClient _httpClient)
{
    private readonly HttpClient httpClient = _httpClient;
    public PlannerDto? Planner { get; set; } = new();
    public async Task LoadProgressAsync(string? plannerId)
    {
        var response = await httpClient.GetFromJsonAsync<PlannerDto>($"api/planner/{plannerId}");
        Planner = response;
    }

    internal async Task SaveProgressAsync()
    {
        var response = await httpClient.PostAsJsonAsync($"api/planner/save", Planner);
        bool isSuccessful = (response.IsSuccessStatusCode);
    }
}

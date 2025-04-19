
using eSUP.DTO;
using System.Net.Http.Json;

namespace eSUP.Client.ViewModels;

public class AssignmentViewModel(HttpClient _httpClient)
{
    private readonly HttpClient httpClient = _httpClient;
    public AssignmentDto? Assignment { get; set; }
    public async Task LoadAssignmentAsync(string? plannerId)
    {
        var response = await httpClient.GetFromJsonAsync<AssignmentDto>($"api/planner/assign/{plannerId}");
        Assignment = response;
    }

    internal async Task SaveAssignmentAsync(string? plannerId)
    {
        var response = await httpClient.PostAsJsonAsync($"api/planner/assign/{plannerId}", Assignment);
        bool isSuccessful = (response.IsSuccessStatusCode);
    }

}

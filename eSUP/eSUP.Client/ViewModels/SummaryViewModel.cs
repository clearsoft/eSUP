using eSUP.DTO;
using System.Net.Http.Json;

namespace eSUP.Client.ViewModels;

public class SummaryViewModel(HttpClient _httpClient)
{
    private readonly HttpClient httpClient = _httpClient;
    public PlannerProgressDto ProgressData { get; set; }
    public int ColumnCount { get; set; } = 20;

    internal async Task LoadSummaryAsync(string plannerId)
    {
        var result = await httpClient.GetFromJsonAsync<PlannerProgressDto>($"api/planner/summary/{plannerId}");
        if(result is not null)
            ProgressData = result;
    }
}

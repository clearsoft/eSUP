using eSUP.DTO;
using System.Net.Http.Json;

namespace eSUP.Client.ViewModels;

public class ProgressViewModel(HttpClient _httpClient)
{
    private readonly HttpClient httpClient = _httpClient;
    public PlannerProgressDto? ProgressData { get; set; }
    public int ColumnCount { get; set; } = 20;

    internal async Task<List<QuestionDto>?> GetDetailAsync(Guid exerciseId, Guid userId)
    {
        try
        {
            var result = await httpClient.GetFromJsonAsync<List<QuestionDto>>($"api/planner/detail/{exerciseId}/{userId}");
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }

    internal async Task LoadSummaryAsync(string plannerId)
    {
        try
        {
            var result = await httpClient.GetFromJsonAsync<PlannerProgressDto>($"api/planner/summary/{plannerId}");
            if (result is not null)
                ProgressData = result;

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}

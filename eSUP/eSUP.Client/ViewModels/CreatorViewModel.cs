using eSUP.DTO;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace eSUP.Client.ViewModels;

public class CreatorViewModel(HttpClient _httpClient)
{
    private readonly HttpClient httpClient = _httpClient;
    private string? selectedOption;

    public PlannerDto? Planner { get; set; } = new PlannerDto();
    public bool IsTrimMode { get; set; } = false;
    public string? SelectedOption { get => selectedOption; set { selectedOption = value; LevelChanged(value); } }

    public void LevelChanged(string? level)
    {
        foreach (var part in Planner!.Exercises.SelectMany(e => e.Questions).SelectMany(q => q.Parts))
        {
            part.IsEnabled = IsPartActive(part, Convert.ToInt32(level));
        }
    }
    private static bool IsPartActive(PartDto part, int level) => part.IsLevelBelow && level == -1 || part.IsLevelAt && level == 0 || part.IsLevelAbove && level == 1;

    internal async Task SavePlannerAsync()
    {
        await httpClient.PostAsJsonAsync("api/planner/new", Planner);
    }

    internal async Task UpdatePlannerPartsAsync(Stack<PartDto> parts)
    {
        await httpClient.PutAsJsonAsync("/api/planner/parts", parts);
    }

    internal async Task RemovePlannerPartsAsync(Stack<PartDto> parts)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, "/api/planner/parts");
        request.Content = new StringContent(JsonSerializer.Serialize(parts), Encoding.UTF8, "application/json");
        await _httpClient.SendAsync(request);
    }

    internal async Task RemovePlannerQuestionsAsync(Stack<QuestionDto> questions)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, "/api/planner/questions");
        request.Content = new StringContent(JsonSerializer.Serialize(questions), Encoding.UTF8, "application/json");
        await _httpClient.SendAsync(request);
    }

    internal async Task RenamePlanner()
    {
        await httpClient.PostAsJsonAsync($"api/planner/rename/{Planner!.Id}", Planner!.Title);
    }

    internal async Task LoadPlannerAsync(string? plannerId)
    {
        var planner = await httpClient.GetFromJsonAsync<PlannerDto>($"api/planner/{plannerId}");
        if (planner is not null)
        {
            Planner = planner;
            Planner.Exercises.ForEach(e => e.LevelSelected = "0");
            SelectedOption = "0";
        }
    }
}

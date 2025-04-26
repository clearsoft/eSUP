using eSUP.DTO;
using System.Net.Http.Json;

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

    internal async Task UpdatePlannerAsync(Stack<PartDto> changes)
    {
        await httpClient.PostAsJsonAsync($"api/planner/update", changes);
        await httpClient.PostAsJsonAsync($"api/planner/rename/{Planner!.Id}", Planner!.Title);
        //if (!response.IsSuccessStatusCode)
        //{
        //    string? error = response.ReasonPhrase;
        //}
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

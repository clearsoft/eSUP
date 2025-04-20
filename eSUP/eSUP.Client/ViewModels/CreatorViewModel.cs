using eSUP.DTO;
using System.Collections.ObjectModel;
using System.Net.Http.Json;

namespace eSUP.Client.ViewModels;

public class CreatorViewModel(HttpClient _httpClient)
{
    private readonly HttpClient httpClient = _httpClient;
    private string? selectedOption;

    public PlannerDto? Planner { get; set; } = new PlannerDto();

    public string? SelectedOption { get => selectedOption; set { selectedOption = value; LevelChanged(value); } }
    internal void CreateNewPlannerAsync(PlannerSpecificationDto dto)
    {
        Planner = Utilities.GenerateNewSUP(dto);
        Planner.Exercises.ForEach(e => e.LevelSelected = "0");
    }

    public void LevelChanged(string? level)
    {
        foreach (var part in Planner!.Exercises.SelectMany(e => e.Questions).SelectMany(q => q.Parts))
        {
            part.IsEnabled = IsPartActive(part, Convert.ToInt32(level));
        }
    }
    private bool IsPartActive(PartDto part, int level) => part.IsLevelBelow && level == -1 || part.IsLevelAt && level == 0 || part.IsLevelAbove && level == 1;

    internal async Task SavePlannerAsync()
    {
        var response = await httpClient.PostAsJsonAsync("api/planner/save", Planner);
    }

    internal async Task UpdatePlannerAsync(Stack<PartDto> changes)
    {
        var response = await httpClient.PostAsJsonAsync($"api/planner/update", changes);
        if (!response.IsSuccessStatusCode)
        {
            string? error = response.ReasonPhrase;
        }
    }

    internal async Task LoadPlannerAsync(string? plannerId)
    {
        var planner = await httpClient.GetFromJsonAsync<PlannerDto>($"api/planner/{plannerId}");
        if (planner is not null)
        {
            Planner = planner;
        }
    }
}

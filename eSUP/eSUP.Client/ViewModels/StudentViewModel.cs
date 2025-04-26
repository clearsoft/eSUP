using eSUP.DTO;
using System.Net.Http.Json;

namespace eSUP.Client.ViewModels;

public class StudentViewModel(HttpClient _httpClient)
{
    private readonly HttpClient httpClient = _httpClient;
    public PlannerDto? Planner { get; set; } = new();
    private string? selectedOption;
    public string? SelectedOption { get => selectedOption; set { selectedOption = value; LevelChanged(value); } }

    public async Task LoadProgressAsync(string? plannerId)
    {
        var response = await httpClient.GetFromJsonAsync<PlannerDto>($"api/planner/{plannerId}");
        Planner = response;
        Planner!.Exercises.ForEach(e => e.LevelSelected = "0");
        SelectedOption = "0";
    }

    public void LevelChanged(string? level)
    {
        foreach (var part in Planner!.Exercises.SelectMany(e => e.Questions).SelectMany(q => q.Parts))
        {
            part.IsEnabled = IsPartActive(part, Convert.ToInt32(level));
        }
    }
    private static bool IsPartActive(PartDto part, int level) => part.IsLevelBelow && level == -1 || part.IsLevelAt && level == 0 || part.IsLevelAbove && level == 1;


    internal async Task SaveProgressAsync()
    {
        await httpClient.PostAsJsonAsync($"api/planner/save", Planner);
        //bool isSuccessful = (response.IsSuccessStatusCode);
    }
}

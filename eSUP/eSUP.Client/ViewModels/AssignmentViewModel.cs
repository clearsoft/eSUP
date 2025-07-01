
using eSUP.Client.Pages;
using eSUP.DTO;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.IO.MemoryMappedFiles;
using System.Net.Http.Json;
using static MudBlazor.CategoryTypes;

namespace eSUP.Client.ViewModels;

public class AssignmentViewModel(HttpClient _httpClient)
{
    private readonly HttpClient httpClient = _httpClient;
    public AssignmentDto? Assignment { get; set; }
    public HashSet<UserInformationDto>? SelectedUsers = [];
    public string? PlannerId { get; set; }
    public string? SearchString { get; set; } = null;
    public string Title => Assignment is not null && Assignment.Title is not null ? Assignment.Title : "-";

    public async Task<List<UserInformationDto>> LoadAssignmentAsync(string? plannerId)
    {
        var response = await httpClient.GetFromJsonAsync<AssignmentDto>($"api/planner/assign/{plannerId}");
        Assignment = response;
        if (Assignment is null)
            return new List<UserInformationDto>(); //Empty list (could happen!)
        return Assignment.Users.Where(u => u.IsAssigned).ToList();
    }

    public async Task<GridData<UserInformationDto>> LoadUsersFromServerAsync(GridState<UserInformationDto> state)
    {
        if (PlannerId is null)
            return new GridData<UserInformationDto>();
        var response = await httpClient.GetFromJsonAsync<AssignmentDto>($"api/planner/assign/{PlannerId}");
        Assignment = response;
        var users = Assignment!.Users;

        await Task.Delay(300);
        users = users.Where(user =>
        {
            if (string.IsNullOrWhiteSpace(SearchString))
                return true;
            if ($"{(user.FirstName ?? "").ToUpper()} {(user.LastName  ?? "").ToUpper()} {(user.Group ?? "").ToUpper()}".Contains(SearchString.ToUpper()))
                return true;
            return false;
        }).ToList();
        var totalItems = users.Count(); 
        
        var sortDefinition = state.SortDefinitions.FirstOrDefault();
        if (sortDefinition is not null && sortDefinition.SortBy == nameof(UserInformationDto.Group))
            users = users.OrderByDirection(sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending, o => o.Group).ToList();
        var pagedUsers = users.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();
        GridData<UserInformationDto> data = new() { Items = pagedUsers, TotalItems = totalItems };
        data.Items.Where(u => u.IsAssigned).ToList().ForEach(u => SelectedUsers!.Add(u));
        return data;
    }

    internal async Task<bool> SaveAssignmentAsync()
    {
        var ids = SelectedUsers!.Select(u => u.UserId).ToList();
        foreach (var user in Assignment!.Users)
        {
            bool isAssigned = ids.Contains(user.UserId);
            user.IsAssigned = isAssigned;
        }
        var response = await httpClient.PostAsJsonAsync($"api/planner/assign/{PlannerId}", Assignment);
        return response.IsSuccessStatusCode;
    }

}

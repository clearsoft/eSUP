using eSUP.DTO;
using Microsoft.JSInterop.Infrastructure;
using System.Collections.ObjectModel;
using System.Net.Http.Json;

namespace eSUP.Client.ViewModels;

public class UserManagementViewModel(HttpClient _httpClient)
{
    private readonly HttpClient httpClient = _httpClient;

    public ObservableCollection<UserInformationDto> Users { get; set; } = [];

    public async Task LoadUsersAsync()
    {
        var users = await httpClient.GetFromJsonAsync<List<UserInformationDto>>("/api/users");
        if (users is not null)
            Users = new ObservableCollection<UserInformationDto>(users);
    }

    public async Task UpgradeRoleAsync(UserInformationDto dto)
    {
        if (dto.Role == "Admin")
            return;

        if (dto.Role == "Teacher")
        {
            var result = await httpClient.GetFromJsonAsync<UserInformationDto>($"/api/users/downgrade/{dto.UserId}");
            if (result is not null)
                dto.Role = "";
        }
        else
        { 
            var result = await httpClient.GetFromJsonAsync<UserInformationDto>($"/api/users/upgrade/{dto.UserId}");
            if (result is not null)
                dto.Role = "Teacher";
        }
    }
}


using CsvHelper;
using eSUP.DTO;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop.Infrastructure;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;

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

    public async Task UploadUserListAsync(IBrowserFile file)
    {
        if(file.ContentType != "text/csv")
            throw new InvalidOperationException("Invalid file type. Please upload a CSV file.");

        using var stream = new MemoryStream();
        await file.OpenReadStream().CopyToAsync(stream);
        stream.Position = 0; // Reset the stream position to the beginning
        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var userList = csv.GetRecords<UserInformationDto>().ToList();
        
        var result = await httpClient.PostAsJsonAsync("/api/users/upload", userList);
    }
}


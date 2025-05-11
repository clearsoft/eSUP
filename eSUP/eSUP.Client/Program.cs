using eSUP.Client.ViewModels;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

namespace eSUP.Client;

internal class Program
{
    static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);

        builder.Services.AddAuthorizationCore();
        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddAuthenticationStateDeserialization();

        builder.Services.AddMudServices();

        builder.Services.AddSingleton(sp =>
            new HttpClient
            {
                BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
            });

        builder.Services.AddSingleton<CreatorViewModel>();
        builder.Services.AddSingleton<StudentViewModel>();
        builder.Services.AddSingleton<ProgressViewModel>();
        builder.Services.AddSingleton<UserManagementViewModel>();
        builder.Services.AddSingleton<PlannerViewModel>();
        builder.Services.AddSingleton<AssignmentViewModel>();

        var app = builder.Build();

        await app.RunAsync();
    }
}

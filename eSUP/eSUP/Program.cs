using eSUP.API;
using eSUP.Client.ViewModels;
using eSUP.Components;
using eSUP.Components.Account;
using eSUP.Model;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Syncfusion.Blazor;

namespace eSUP;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents()
            .AddAuthenticationStateSerialization();

        builder.Services.AddSyncfusionBlazor();

        builder.Services.AddMudServices();

        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddScoped<IdentityUserAccessor>();
        builder.Services.AddScoped<IdentityRedirectManager>();
        builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddIdentityCookies();

        //string? googleClientId = builder.Configuration.GetValue<string>("Authentication:Google:ClientId");
        //string? googleClientSecret = builder.Configuration.GetValue<string>("Authentication:Google:ClientSecret");
        //builder.Services.AddAuthentication().AddGoogle(googleOptions =>
        //{
        //    googleOptions.ClientId =
        //    builder.Configuration.GetValue<string>(googleClientId);

        //    googleOptions.ClientSecret =
        //    builder.Configuration.GetValue<string>(googleClientSecret);
        //});

        //var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        //builder.Services.AddDbContext<MainContext>(options => options.UseSqlite(connectionString));
        builder.Services.AddDbContext<MainContext>(options => options.UseSqlite("Data Source = eSUP.db"));
        // Local: ./Data/eSUP.db
        // On Azure: D:/home/site/wwwroot/Data/eSUP.db
        //string connectionString = "Data Source=./Data/eSUP.db";
        string connectionString = "Data Source=/src/eSUP/eSUP/Data/eSUP.db";
        builder.Services.AddDbContext<MainContext>(options => options.UseSqlite(connectionString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();
        builder.Services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 5;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
        });
        builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<MainContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<MainContext>();
            db.Database.Migrate();
        }

        app.UseHttpsRedirection();

        app.UseAntiforgery();

        app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

        // Add additional endpoints required by the Identity /Account Razor components.
        app.MapAdditionalIdentityEndpoints();

        app.MapClientAPI();

        app.Run();
    }
}

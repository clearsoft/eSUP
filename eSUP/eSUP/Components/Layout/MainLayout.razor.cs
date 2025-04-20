using Microsoft.AspNetCore.Components.Authorization;

namespace eSUP.Components.Layout;

public partial class MainLayout(AuthenticationStateProvider _auth)
{
    AuthenticationStateProvider auth => _auth;
    private string welcomeMessage = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        var authState = await auth.GetAuthenticationStateAsync();
        if (authState != null)
        {
            var user = authState.User;
            if (user.Identity?.IsAuthenticated == true)
            {
                var userName = user.Identity.Name ?? string.Empty;
                welcomeMessage = "Welcome " + userName;
            }
            else
            {
                welcomeMessage = "Log in to begin";
            }
        }
        await base.OnInitializedAsync();
    }
}

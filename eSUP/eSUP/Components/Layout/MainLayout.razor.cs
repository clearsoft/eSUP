using Microsoft.AspNetCore.Components.Authorization;

namespace eSUP.Components.Layout;

public partial class MainLayout(AuthenticationStateProvider _auth)
{
    private AuthenticationStateProvider Auth => _auth;
    private string welcomeMessage = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        var authState = await Auth.GetAuthenticationStateAsync();
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

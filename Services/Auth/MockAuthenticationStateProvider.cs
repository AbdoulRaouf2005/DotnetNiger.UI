using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using DotnetNiger.UI.Models.Responses;

namespace DotnetNiger.UI.Services.Auth;

public class MockAuthenticationStateProvider : AuthenticationStateProvider
{
    private static readonly AuthenticationState Anonymous =
        new(new ClaimsPrincipal(new ClaimsIdentity()));

    private AuthenticationState _currentState = Anonymous;

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return Task.FromResult(_currentState);
    }

    public void SetAuthenticated(UserDto user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.FullName ?? user.Username ?? ""),
            new(ClaimTypes.Email, user.Email ?? ""),
        };

        if (user.Roles is not null)
        {
            foreach (var role in user.Roles)
                claims.Add(new Claim(ClaimTypes.Role, role));
        }

        if (!string.IsNullOrWhiteSpace(user.AvatarUrl))
            claims.Add(new Claim("avatar_url", user.AvatarUrl));

        var identity = new ClaimsIdentity(claims, "mock");
        _currentState = new AuthenticationState(new ClaimsPrincipal(identity));
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public void SetAnonymous()
    {
        _currentState = Anonymous;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}

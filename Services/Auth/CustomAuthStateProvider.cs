using System.Security.Claims;
using DotnetNiger.UI.Models.Responses;
using Microsoft.AspNetCore.Components.Authorization;

namespace DotnetNiger.UI.Services.Auth;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private static readonly AuthenticationState Anonymous =
        new(new ClaimsPrincipal(new ClaimsIdentity()));

    private AuthenticationState _cachedState = Anonymous;
    private DateTime _cacheExpiry = DateTime.MinValue;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);

    private readonly TokenStorageService _tokenStorage;

    public CustomAuthStateProvider(TokenStorageService tokenStorage)
    {
        _tokenStorage = tokenStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (_cachedState != Anonymous && DateTime.UtcNow < _cacheExpiry)
            return _cachedState;

        var state = await RestoreFromTokenAsync();
        if (state != Anonymous)
        {
            _cachedState = state;
            _cacheExpiry = DateTime.UtcNow.Add(CacheDuration);
        }

        return state;
    }

    private async Task<AuthenticationState> RestoreFromTokenAsync()
    {
        var accessToken = await _tokenStorage.GetAccessTokenAsync();
        if (string.IsNullOrWhiteSpace(accessToken))
            return Anonymous;

        var claims = AuthService.ParseClaimsFromJwt(accessToken).ToList();
        if (claims.Count == 0)
            return Anonymous;

        return new AuthenticationState(
            new ClaimsPrincipal(new ClaimsIdentity(claims, "cookie")));
    }

    public void SetAuthenticatedFromUser(UserDto user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.FullName),
            new("full_name", user.FullName),
            new("avatar_url", user.AvatarUrl),
        };
        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
            if (role.Length > 0)
            {
                var capped = char.ToUpperInvariant(role[0]) + role[1..];
                if (capped != role)
                    claims.Add(new Claim(ClaimTypes.Role, capped));
            }
        }

        _cachedState = new AuthenticationState(
            new ClaimsPrincipal(new ClaimsIdentity(claims, "cookie")));
        _cacheExpiry = DateTime.UtcNow.Add(CacheDuration);
        NotifyAuthenticationStateChanged(Task.FromResult(_cachedState));
    }

    public void NotifyStateChanged()
    {
        _cachedState = Anonymous;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

}

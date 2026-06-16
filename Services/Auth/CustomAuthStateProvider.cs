using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using DotnetNiger.UI.Models.Responses;
using Microsoft.AspNetCore.Components.Authorization;

namespace DotnetNiger.UI.Services.Auth;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _http;
    private static readonly AuthenticationState Anonymous =
        new(new ClaimsPrincipal(new ClaimsIdentity()));

    private AuthenticationState _cachedState = Anonymous;
    private DateTime _cacheExpiry = DateTime.MinValue;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    public CustomAuthStateProvider(HttpClient http)
    {
        _http = http;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (_cachedState != Anonymous && DateTime.UtcNow < _cacheExpiry)
            return _cachedState;

        try
        {
            var response = await _http.GetAsync("api/auth/session");
            if (!response.IsSuccessStatusCode)
            {
                _cachedState = Anonymous;
                return Anonymous;
            }

            var session = await response.Content.ReadFromJsonAsync<SessionResponse>();
            if (session?.Authenticated == true && session.Claims is { Count: >0 })
            {
            var claims = session.Claims
                .Select(c => new Claim(c.Type,
                    c.Type == ClaimTypes.Role && c.Value.Length > 0
                        ? char.ToUpperInvariant(c.Value[0]) + c.Value[1..]
                        : c.Value))
                .ToList();

                _cachedState = new AuthenticationState(
                    new ClaimsPrincipal(new ClaimsIdentity(claims, "cookie")));
                _cacheExpiry = DateTime.UtcNow.Add(CacheDuration);
                return _cachedState;
            }

            _cachedState = Anonymous;
            return Anonymous;
        }
        catch
        {
            _cachedState = Anonymous;
            return Anonymous;
        }
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
            claims.Add(new Claim(ClaimTypes.Role,
                role.Length > 0 ? char.ToUpperInvariant(role[0]) + role[1..] : role));

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

    private sealed record SessionResponse(bool Authenticated, List<ClaimDto>? Claims);
    private sealed record ClaimDto(string Type, string Value);
}

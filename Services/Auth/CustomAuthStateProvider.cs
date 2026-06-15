using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using DotnetNiger.UI.Services.Contracts;

namespace DotnetNiger.UI.Services.Auth;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly IJSRuntime _js;
    private readonly IServiceProvider _serviceProvider;
    private static readonly AuthenticationState Anonymous =
        new(new ClaimsPrincipal(new ClaimsIdentity()));

    private bool _isRefreshing;

    public CustomAuthStateProvider(IJSRuntime js, IServiceProvider serviceProvider)
    {
        _js = js;
        _serviceProvider = serviceProvider;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await GetAccessTokenAsync();
        if (string.IsNullOrWhiteSpace(token))
            return Anonymous;

        var claims = ParseClaimsFromJwt(token);
        var expClaim = claims.FirstOrDefault(c => c.Type == "exp")?.Value;

        if (expClaim != null && long.TryParse(expClaim, out var expUnix))
        {
            var expDate = DateTimeOffset.FromUnixTimeSeconds(expUnix);
            if (expDate <= DateTimeOffset.UtcNow && !_isRefreshing)
            {
                _isRefreshing = true;
                try
                {
                    var authService = _serviceProvider.GetRequiredService<IAuthService>();
                    var refreshed = await authService.RefreshTokenAsync();
                    if (refreshed?.Token?.AccessToken is not null)
                    {
                        token = refreshed.Token.AccessToken;
                        claims = ParseClaimsFromJwt(token);
                    }
                    else
                    {
                        await ClearTokensAsync();
                        return Anonymous;
                    }
                }
                catch
                {
                    await ClearTokensAsync();
                    return Anonymous;
                }
                finally
                {
                    _isRefreshing = false;
                }
            }
        }

        var identity = new ClaimsIdentity(claims, "jwt");
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public async Task<string?> GetAccessTokenAsync()
        => await _js.InvokeAsync<string?>("localStorage.getItem", "accessToken");

    public async Task<string?> GetRefreshTokenAsync()
        => await _js.InvokeAsync<string?>("localStorage.getItem", "refreshToken");

    public async Task SaveTokensAsync(string accessToken, string refreshToken)
    {
        await _js.InvokeVoidAsync("localStorage.setItem", "accessToken", accessToken);
        await _js.InvokeVoidAsync("localStorage.setItem", "refreshToken", refreshToken);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task ClearTokensAsync()
    {
        await _js.InvokeVoidAsync("localStorage.removeItem", "accessToken");
        await _js.InvokeVoidAsync("localStorage.removeItem", "refreshToken");
        NotifyAuthenticationStateChanged(Task.FromResult(Anonymous));
    }

    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var kvs = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonBytes)!;

        return kvs.SelectMany(kv =>
        {
            if (kv.Key is "roles" or "role")
            {
                if (kv.Value.ValueKind == JsonValueKind.Array)
                    return kv.Value.EnumerateArray().Select(r => new Claim(ClaimTypes.Role, r.GetString()!));
                return new[] { new Claim(ClaimTypes.Role, kv.Value.GetString()!) };
            }
            return new[] { new Claim(kv.Key, kv.Value.ToString()) };
        });
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        base64 = base64.Replace('-', '+').Replace('_', '/');
        return (base64.Length % 4) switch
        {
            2 => Convert.FromBase64String(base64 + "=="),
            3 => Convert.FromBase64String(base64 + "="),
            _ => Convert.FromBase64String(base64),
        };
    }
}

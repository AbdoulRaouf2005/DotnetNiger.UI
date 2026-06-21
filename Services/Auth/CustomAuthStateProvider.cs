using System.Security.Claims;
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

        var claims = JwtParser.ParseClaimsFromJwt(token);
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
                        claims = JwtParser.ParseClaimsFromJwt(token);
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
    {
        try
        {
            return await _js.InvokeAsync<string?>("localStorage.getItem", "accessToken");
        }
        catch
        {
            return null;
        }
    }

    public async Task<string?> GetRefreshTokenAsync()
    {
        try
        {
            return await _js.InvokeAsync<string?>("sessionStorage.getItem", "refreshToken");
        }
        catch
        {
            return null;
        }
    }

    public async Task SaveTokensAsync(string accessToken, string refreshToken)
    {
        try
        {
            await _js.InvokeVoidAsync("localStorage.setItem", "accessToken", accessToken);
            await _js.InvokeVoidAsync("sessionStorage.setItem", "refreshToken", refreshToken);
        }
        catch
        {
            // JS interop may fail during pre-rendering
        }
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task ClearTokensAsync()
    {
        try
        {
            await _js.InvokeVoidAsync("localStorage.removeItem", "accessToken");
            await _js.InvokeVoidAsync("sessionStorage.removeItem", "refreshToken");
        }
        catch
        {
            // JS interop may fail during pre-rendering
        }
        NotifyAuthenticationStateChanged(Task.FromResult(Anonymous));
    }

}

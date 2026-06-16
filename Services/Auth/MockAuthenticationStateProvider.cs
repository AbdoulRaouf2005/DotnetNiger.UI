using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace DotnetNiger.UI.Services.Auth;

public class MockAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly AuthenticationState _anonymousState;

    public MockAuthenticationStateProvider(AuthenticationState anonymousState)
    {
        _anonymousState = anonymousState;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return Task.FromResult(_anonymousState);
    }
}

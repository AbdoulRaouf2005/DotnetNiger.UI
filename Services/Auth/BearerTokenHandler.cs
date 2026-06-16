using System.Net.Http.Headers;

namespace DotnetNiger.UI.Services.Auth;

public class BearerTokenHandler : DelegatingHandler
{
    private readonly TokenStorageService _tokenStorage;

    public BearerTokenHandler(TokenStorageService tokenStorage)
    {
        _tokenStorage = tokenStorage;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Headers.Authorization is null)
        {
            var token = await _tokenStorage.GetAccessTokenAsync();
            if (token is not null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}

using System.Net;
using System.Net.Http.Headers;
using DotnetNiger.UI.Services.Contracts;

namespace DotnetNiger.UI.Services.Auth;

public class ClientIdHeaderHandler : DelegatingHandler
{
    private readonly ClientIdentifierProvider _clientIdentifierProvider;
    private readonly CustomAuthStateProvider _authStateProvider;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ClientIdHeaderHandler> _logger;

    public ClientIdHeaderHandler(
        ClientIdentifierProvider clientIdentifierProvider,
        CustomAuthStateProvider authStateProvider,
        IServiceProvider serviceProvider,
        ILogger<ClientIdHeaderHandler> logger)
    {
        _clientIdentifierProvider = clientIdentifierProvider;
        _authStateProvider = authStateProvider;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!request.Headers.Contains("ClientId"))
        {
            var clientId = await _clientIdentifierProvider.GetClientIdAsync();
            request.Headers.TryAddWithoutValidation("ClientId", clientId);
            _logger.LogDebug("Header ClientId injecte: {ClientId} sur {Method} {Uri}", clientId, request.Method, request.RequestUri);
        }

        if (request.Headers.Authorization is null)
        {
            var token = await _authStateProvider.GetAccessTokenAsync();
            if (!string.IsNullOrWhiteSpace(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized
            && request.Headers.Authorization != null
            && request.RequestUri != null
            && !request.RequestUri.AbsolutePath.Contains("connect/token"))
        {
            _logger.LogInformation("Tentative de rafraichissement du token apres 401 sur {Method} {Uri}", request.Method, request.RequestUri);

            try
            {
                var authService = _serviceProvider.GetRequiredService<IAuthService>();
                var refreshed = await authService.RefreshTokenAsync();

                if (refreshed?.Token?.AccessToken is not null)
                {
                    var clone = await CloneRequestAsync(request, cancellationToken);
                    clone.Headers.Authorization = new AuthenticationHeaderValue("Bearer", refreshed.Token.AccessToken);
                    response = await base.SendAsync(clone, cancellationToken);
                    _logger.LogInformation("Requete reessayee avec succes apres rafraichissement du token");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Echec du rafraichissement du token: {Message}", ex.Message);
            }
        }

        return response;
    }

    private static async Task<HttpRequestMessage> CloneRequestAsync(HttpRequestMessage request, CancellationToken ct)
    {
        var clone = new HttpRequestMessage(request.Method, request.RequestUri);

        if (request.Content != null)
        {
            var contentBytes = await request.Content.ReadAsByteArrayAsync(ct);
            clone.Content = new ByteArrayContent(contentBytes);
            foreach (var header in request.Content.Headers)
                clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        foreach (var header in request.Headers)
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

        clone.Version = request.Version;

        return clone;
    }
}

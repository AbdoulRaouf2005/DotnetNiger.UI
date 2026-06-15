using System.Net;

namespace DotnetNiger.UI.Services.Auth;

public class ClientIdHeaderHandler : DelegatingHandler
{
    private readonly ClientIdentifierProvider _clientIdentifierProvider;
    private readonly ILogger<ClientIdHeaderHandler> _logger;

    public ClientIdHeaderHandler(
        ClientIdentifierProvider clientIdentifierProvider,
        ILogger<ClientIdHeaderHandler> logger)
    {
        _clientIdentifierProvider = clientIdentifierProvider;
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

        var response = await base.SendAsync(request, cancellationToken);
        return response;
    }
}

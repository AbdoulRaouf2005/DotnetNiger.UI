using Microsoft.JSInterop;
using Microsoft.Extensions.Logging;

namespace DotnetNiger.UI.Services.Auth;

public class ClientIdentifierProvider
{
    private const string StorageKey = "clientId";
    private string? _cachedClientId;
    private readonly IJSRuntime _js;
    private readonly ILogger<ClientIdentifierProvider> _logger;

    public ClientIdentifierProvider(IJSRuntime js, ILogger<ClientIdentifierProvider> logger)
    {
        _js = js;
        _logger = logger;
    }

    public async Task<string> GetClientIdAsync()
    {
        if (!string.IsNullOrWhiteSpace(_cachedClientId))
        {
            _logger.LogDebug("ClientId récupéré depuis le cache mémoire: {ClientId}", _cachedClientId);
            return _cachedClientId;
        }

        var existing = await _js.InvokeAsync<string?>("localStorage.getItem", StorageKey);
        if (!string.IsNullOrWhiteSpace(existing))
        {
            _cachedClientId = existing;
            _logger.LogInformation("ClientId existant restauré depuis localStorage: {ClientId}", existing);
            return existing;
        }

        var generated = $"web-{Guid.NewGuid():D}";
        await _js.InvokeVoidAsync("localStorage.setItem", StorageKey, generated);
        _cachedClientId = generated;
        _logger.LogInformation("Nouveau ClientId généré et persisté: {ClientId}", generated);
        return generated;
    }
}

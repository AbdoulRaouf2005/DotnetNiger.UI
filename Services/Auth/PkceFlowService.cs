using System.Security.Cryptography;

namespace DotnetNiger.UI.Services.Auth;

public class PkceFlowService
{
    private readonly ProtectedStorageService _protectedStorage;
    private readonly IConfiguration _config;

    public PkceFlowService(ProtectedStorageService protectedStorage, IConfiguration config)
    {
        _protectedStorage = protectedStorage;
        _config = config;
    }

    public async Task<string> BuildLoginUrlAsync(string baseUri, string? callbackPath = null)
    {
        var state = GenerateState();
        await _protectedStorage.SavePkceStateAsync(string.Empty, state);

        var apiBase = _config["ApiBaseUrl"] ?? baseUri.TrimEnd('/');
        callbackPath ??= _config["IdentityEndpoints:CallbackPath"] ?? "/auth/external-callback";
        var returnUrl = $"{baseUri.TrimEnd('/')}{callbackPath}?state={state}";
        var path = _config["IdentityEndpoints:Login"] ?? "/Account/Login";

        return $"{apiBase}{path}?returnUrl={Uri.EscapeDataString(returnUrl)}";
    }

    public async Task<string?> ValidateStateAsync(string state)
    {
        return await _protectedStorage.GetAndClearPkceVerifierAsync(state);
    }

    private static string GenerateState()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}

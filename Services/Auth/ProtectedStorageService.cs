using DotnetNiger.UI.Services.Contracts;

namespace DotnetNiger.UI.Services.Auth;

public class ProtectedStorageService
{
    private readonly ILocalStorageService _storage;
    private const string AccessTokenKey = "access_token";
    private const string RefreshTokenKey = "refresh_token";
    private const string PkceVerifierKey = "pkce_code_verifier";
    private const string PkceStateKey = "pkce_state";

    public ProtectedStorageService(ILocalStorageService storage)
    {
        _storage = storage;
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        return await _storage.GetItemAsync<string>(AccessTokenKey);
    }

    public async Task<string?> GetRefreshTokenAsync()
    {
        return await _storage.GetItemAsync<string>(RefreshTokenKey);
    }

    public async Task SaveTokensAsync(string accessToken, string? refreshToken)
    {
        await _storage.SetItemAsync(AccessTokenKey, accessToken);
        if (refreshToken is not null)
            await _storage.SetItemAsync(RefreshTokenKey, refreshToken);
    }

    public async Task ClearTokensAsync()
    {
        await _storage.RemoveItemAsync(AccessTokenKey);
        await _storage.RemoveItemAsync(RefreshTokenKey);
    }

    public async Task SavePkceStateAsync(string verifier, string state)
    {
        await _storage.SetItemAsync(PkceVerifierKey, verifier);
        await _storage.SetItemAsync(PkceStateKey, state);
    }

    public async Task<string?> GetAndClearPkceVerifierAsync(string expectedState)
    {
        var savedState = await _storage.GetItemAsync<string>(PkceStateKey);
        if (savedState != expectedState)
            return null;

        var verifier = await _storage.GetItemAsync<string>(PkceVerifierKey);

        await _storage.RemoveItemAsync(PkceVerifierKey);
        await _storage.RemoveItemAsync(PkceStateKey);

        return verifier;
    }
}

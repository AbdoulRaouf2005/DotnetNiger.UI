using DotnetNiger.UI.Services.Contracts;

namespace DotnetNiger.UI.Services.Auth;

public class TokenStorageService
{
    private readonly ILocalStorageService _storage;
    private const string AccessTokenKey = "access_token";
    private const string RefreshTokenKey = "refresh_token";

    public TokenStorageService(ILocalStorageService storage)
    {
        _storage = storage;
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        return await _storage.GetItemAsync<string>(AccessTokenKey);
    }

    public async Task SaveTokensAsync(string accessToken, string? refreshToken)
    {
        await _storage.SetItemAsync(AccessTokenKey, accessToken);
        if (refreshToken is not null)
            await _storage.SetItemAsync(RefreshTokenKey, refreshToken);
    }

    public async Task ClearAsync()
    {
        await _storage.RemoveItemAsync(AccessTokenKey);
        await _storage.RemoveItemAsync(RefreshTokenKey);
    }
}

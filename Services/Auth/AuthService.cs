using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using DotnetNiger.UI.Helpers;
using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Models.Responses;
using DotnetNiger.UI.Services.Api;
using DotnetNiger.UI.Services.Contracts;

namespace DotnetNiger.UI.Services.Auth;

public class AuthService : IAuthService
{
    private readonly HttpClient _http;
    private readonly CustomAuthStateProvider _authProvider;
    private readonly IUserStateService _userStateService;
    private readonly string _clientId;

    public AuthService(HttpClient http, CustomAuthStateProvider authProvider, IUserStateService userStateService, string clientId = "web-ui")
    {
        _http = http;
        _authProvider = authProvider;
        _userStateService = userStateService;
        _clientId = clientId;
    }

    public async Task<ApiSuccessResponse<AuthDto>> LoginAsync(LoginRequest request)
    {
        try
        {
            var formData = new Dictionary<string, string>
            {
                ["grant_type"] = "password",
                ["username"] = request.Email,
                ["password"] = request.Password,
                ["scope"] = "openid profile email roles offline_access",
                ["client_id"] = _clientId
            };

            var response = await _http.PostAsync(ApiEndpoints.Auth.Token, new FormUrlEncodedContent(formData));

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                var message = TryReadOidcError(errorBody);

                return new ApiSuccessResponse<AuthDto>
                {
                    Success = false,
                    Message = message ?? $"Connexion impossible (HTTP {(int)response.StatusCode})."
                };
            }

            var (authDto, error) = await ParseTokenResponseAsync(response);
            if (authDto is not null)
            {
                if (authDto.Token is not null)
                    await _authProvider.SaveTokensAsync(authDto.Token.AccessToken, authDto.Token.RefreshToken);
                return new ApiSuccessResponse<AuthDto> { Success = true, Data = authDto };
            }

            return new ApiSuccessResponse<AuthDto> { Success = false, Message = error ?? "Erreur de connexion." };
        }
        catch (HttpRequestException ex)
        {
            return new()
            {
                Success = false,
                Message = ex.Message
            };
        }
        catch (TaskCanceledException)
        {
            return new ApiSuccessResponse<AuthDto>
            {
                Success = false,
                Message = "Le serveur a mis trop de temps à répondre."
            };
        }
    }

    public string? GetRoleFromAccessToken(string? accessToken)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
            return null;

        var segments = accessToken.Split('.');
        if (segments.Length < 2)
            return null;

        try
        {
            var payloadJson = Encoding.UTF8.GetString(JwtParser.ParseBase64WithoutPadding(segments[1]));
            using var document = JsonDocument.Parse(payloadJson);
            var root = document.RootElement;

            if (TryGetRoleValue(root, "roles", out var roleFromRoles))
                return roleFromRoles;

            if (TryGetRoleValue(root, "role", out var roleFromRole))
                return roleFromRole;

            if (TryGetRoleValue(root, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role", out var roleFromClaimType))
                return roleFromClaimType;

            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<ApiSuccessResponse<AuthDto>> CompleteExternalLoginAsync(string ticket)
    {
        try
        {
            var formData = new Dictionary<string, string>
            {
                ["grant_type"] = "external_login",
                ["ticket"] = ticket,
                ["client_id"] = _clientId,
                ["scope"] = "openid profile email roles offline_access"
            };

            var response = await _http.PostAsync(ApiEndpoints.Auth.Token, new FormUrlEncodedContent(formData));

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                var message = TryReadOidcError(errorBody);
                return new ApiSuccessResponse<AuthDto>
                {
                    Success = false,
                    Message = message ?? "Erreur lors de la connexion externe."
                };
            }

            var (authDto, error) = await ParseTokenResponseAsync(response);
            if (authDto is not null)
            {
                if (authDto.Token is not null)
                    await _authProvider.SaveTokensAsync(authDto.Token.AccessToken, authDto.Token.RefreshToken);
                return new ApiSuccessResponse<AuthDto> { Success = true, Data = authDto };
            }

            return new ApiSuccessResponse<AuthDto> { Success = false, Message = error ?? "Erreur de connexion externe." };
        }
        catch (HttpRequestException ex)
        {
            return new() { Success = false, Message = ex.Message };
        }
        catch (TaskCanceledException)
        {
            return new ApiSuccessResponse<AuthDto> { Success = false, Message = "Le serveur a mis trop de temps à répondre." };
        }
    }

    public async Task<ApiSuccessResponse<AuthDto>> RegisterAsync(RegisterRequest request)
    {
        try
        {
            var names = (request.FullName ?? "").Split(' ', 2, StringSplitOptions.TrimEntries);
            var registerPayload = new
            {
                email = request.Email,
                password = request.Password,
                firstName = names.Length > 0 ? names[0] : "",
                lastName = names.Length > 1 ? names[1] : ""
            };

            var response = await _http.PostAsJsonAsync(ApiEndpoints.Auth.Register, registerPayload);
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await TryReadErrorMessageAsync(response.Content);

                return new ApiSuccessResponse<AuthDto>
                {
                    Success = false,
                    Message = !string.IsNullOrWhiteSpace(errorMessage)
                        ? errorMessage
                        : $"Inscription impossible (HTTP {(int)response.StatusCode})."
                };
            }

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var userId = root.TryGetProperty("userId", out var uidProp)
                && uidProp.ValueKind == JsonValueKind.String
                ? uidProp.GetString() : null;
            var email = root.TryGetProperty("email", out var emailProp)
                && emailProp.ValueKind == JsonValueKind.String
                ? emailProp.GetString() : null;
            var message = root.TryGetProperty("message", out var msgProp)
                && msgProp.ValueKind == JsonValueKind.String
                ? msgProp.GetString() : "Compte créé. Vérifiez votre email pour le confirmer.";

            return new ApiSuccessResponse<AuthDto>
            {
                Success = true,
                Message = message,
                Data = new AuthDto
                {
                    User = new UserDto
                    {
                        Id = Guid.TryParse(userId, out var uid) ? uid : Guid.Empty,
                        Email = email ?? request.Email,
                        FullName = request.FullName ?? "",
                        Username = request.FullName ?? ""
                    }
                }
            };
        }
        catch (HttpRequestException ex)
        {
            return new ApiSuccessResponse<AuthDto>
            {
                Success = false,
                Message = ex.Message
            };
        }
        catch (TaskCanceledException)
        {
            return new ApiSuccessResponse<AuthDto>
            {
                Success = false,
                Message = "Le serveur a mis trop de temps à répondre."
            };
        }
    }

    public async Task LogoutAsync()
    {
        var refreshToken = await _authProvider.GetRefreshTokenAsync();

        if (!string.IsNullOrWhiteSpace(refreshToken))
        {
            await _http.PostAsJsonAsync(ApiEndpoints.Auth.Logout,
                new RefreshTokenRequest { RefreshToken = refreshToken, ClientId = _clientId });
        }

        await _authProvider.ClearTokensAsync();
        await _userStateService.ClearUserAsync();
    }

    private static readonly SemaphoreSlim _refreshLock = new(1, 1);

    /// <summary>
    /// Renouvelle l'access token depuis le refresh token stocké.
    /// Efface la session si le refresh token est invalide ou expiré.
    /// </summary>
    public async Task<AuthDto?> RefreshTokenAsync()
    {
        if (!await _refreshLock.WaitAsync(0))
            return null;

        try
        {
            var refreshToken = await _authProvider.GetRefreshTokenAsync();
            if (string.IsNullOrWhiteSpace(refreshToken))
                return null;

            var formData = new Dictionary<string, string>
            {
                ["grant_type"] = "refresh_token",
                ["refresh_token"] = refreshToken,
                ["scope"] = "openid profile email roles offline_access",
                ["client_id"] = _clientId
            };

            var response = await _http.PostAsync(ApiEndpoints.Auth.Token, new FormUrlEncodedContent(formData));

            if (!response.IsSuccessStatusCode)
            {
                await _authProvider.ClearTokensAsync();
                return null;
            }

            var (authDto, _) = await ParseTokenResponseAsync(response);
            if (authDto?.Token is not null)
                await _authProvider.SaveTokensAsync(authDto.Token.AccessToken, authDto.Token.RefreshToken);

            return authDto;
        }
        finally
        {
            _refreshLock.Release();
        }
    }

	public async Task<UserDto?> GetCurrentUserAsync()
	{
		var token = await _authProvider.GetAccessTokenAsync();
		if (string.IsNullOrWhiteSpace(token))
			return null;

		var claims = ParseClaimsFromJwt(token).ToList();
		var userIdClaim = claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier || claim.Type == "sub");

		if (userIdClaim is null || !Guid.TryParse(userIdClaim.Value, out var userId))
			return null;

		var email = claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email || claim.Type == "email")?.Value ?? string.Empty;
		var fullName = claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name || claim.Type == "name" || claim.Type == "full_name")?.Value ?? string.Empty;
		var avatarUrl = claims.FirstOrDefault(claim => claim.Type == "avatar_url" || claim.Type == "avatarUrl" || claim.Type == "picture")?.Value ?? string.Empty;
		var roles = claims
			.Where(claim => claim.Type == ClaimTypes.Role)
			.Select(claim => claim.Value)
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToList();

		return new UserDto
		{
			Id = userId,
			Email = email,
			FullName = fullName,
			AvatarUrl = avatarUrl,
			Username = string.IsNullOrWhiteSpace(fullName) ? email : fullName,
			IsActive = true,
			Roles = roles
		};
	}

    public async Task<bool> IsAuthenticatedAsync()
        => !string.IsNullOrWhiteSpace(await _authProvider.GetAccessTokenAsync());

    public async Task<bool> IsAdminAsync()
    {
        var token = await _authProvider.GetAccessTokenAsync();
        var role = GetRoleFromAccessToken(token);

        return RoleConstants.IsAdminRole(role);
    }

    public Task<string?> GetAccessTokenAsync()
        => _authProvider.GetAccessTokenAsync();

    public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        var response = await _http.PostAsJsonAsync(ApiEndpoints.Auth.ForgotPassword, request);
        return response.IsSuccessStatusCode;
    }

    public async Task<ApiSuccessResponse<object>> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var resetPayload = new { email = request.Email, token = request.Token, password = request.NewPassword };
        var response = await _http.PostAsJsonAsync(ApiEndpoints.Auth.ResetPassword, resetPayload);
        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await TryReadErrorMessageAsync(response.Content);

            return new ApiSuccessResponse<object>
            {
                Success = false,
                Message = !string.IsNullOrWhiteSpace(errorMessage)
                    ? errorMessage
                    : "Erreur lors de la réinitialisation."
            };
        }

        if (response.Content.Headers.ContentLength is null or 0)
        {
            return new ApiSuccessResponse<object>
            {
                Success = true,
                Message = "Mot de passe réinitialisé avec succès."
            };
        }

        var wrapped = await response.Content.ReadFromJsonAsync<ApiSuccessResponse<object>>();
        return new ApiSuccessResponse<object>
        {
            Success = true,
            Message = wrapped?.Message ?? "Mot de passe réinitialisé avec succès."
        };
    }

    public async Task<bool> RequestEmailVerificationAsync(RequestEmailVerificationRequest request)
    {
        var response = await _http.PostAsJsonAsync(ApiEndpoints.Auth.RequestEmailVerification, request);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> VerifyEmailAsync(VerifyEmailRequest request)
    {
        var response = await _http.PostAsJsonAsync(ApiEndpoints.Auth.VerifyEmail, request);
        return response.IsSuccessStatusCode;
    }

    private static async Task<(AuthDto?, string?)> ParseTokenResponseAsync(HttpResponseMessage response)
    {
        try
        {
            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var accessToken = root.GetProperty("access_token").GetString()!;
            var refreshToken = root.TryGetProperty("refresh_token", out var rt) ? rt.GetString() : null;
            var expiresIn = root.TryGetProperty("expires_in", out var exp) ? exp.GetInt32() : 3600;

            var claims = ParseClaimsFromJwt(accessToken).ToList();

            var userId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "sub")?.Value ?? "";
            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email || c.Type == "email")?.Value ?? "";
            var fullName = claims.FirstOrDefault(c => c.Type is "name" or "full_name")?.Value ?? "";
            var avatarUrl = claims.FirstOrDefault(c => c.Type is "avatar_url" or "avatarUrl" or "picture")?.Value ?? "";
            var roles = claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).Distinct(StringComparer.OrdinalIgnoreCase).ToList();

            var user = new UserDto
            {
                Id = Guid.TryParse(userId, out var uid) ? uid : Guid.Empty,
                Email = email,
                FullName = fullName ?? email,
                Username = fullName ?? email,
                AvatarUrl = avatarUrl ?? string.Empty,
                IsActive = true,
                Roles = roles
            };

            var token = new TokenDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken ?? string.Empty,
                TokenType = "Bearer",
                ExpiresIn = expiresIn
            };

            return (new AuthDto { User = user, Token = token }, null);
        }
        catch (Exception ex)
        {
            return (null, $"Erreur de lecture de la réponse: {ex.Message}");
        }
    }

    private static string? TryReadOidcError(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            if (root.TryGetProperty("error_description", out var desc) && desc.ValueKind == JsonValueKind.String)
                return desc.GetString();
            if (root.TryGetProperty("error", out var err) && err.ValueKind == JsonValueKind.String)
                return err.GetString();
        }
        catch { }
        return null;
    }

    private static bool TryGetRoleValue(JsonElement root, string key, out string? role)
    {
        role = null;

        if (!root.TryGetProperty(key, out var roleElement))
            return false;

        if (roleElement.ValueKind == JsonValueKind.Array)
        {
            role = roleElement
                .EnumerateArray()
                .Select(x => x.GetString())
                .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
            return !string.IsNullOrWhiteSpace(role);
        }

        if (roleElement.ValueKind == JsonValueKind.String)
        {
            role = roleElement.GetString();
            return !string.IsNullOrWhiteSpace(role);
        }

        return false;
    }

    private static async Task<string?> TryReadErrorMessageAsync(HttpContent content)
    {
        try
        {
            var json = await content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
                return null;

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.TryGetProperty("detail", out var detail) && detail.ValueKind == JsonValueKind.String)
                return detail.GetString();

            if (root.TryGetProperty("message", out var message) && message.ValueKind == JsonValueKind.String)
                return message.GetString();

            if (root.TryGetProperty("error", out var error) && error.ValueKind == JsonValueKind.String)
                return error.GetString();

            return null;
        }
        catch
        {
            return null;
        }
    }

    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        => JwtParser.ParseClaimsFromJwt(jwt);
}

using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Models.Responses;
using DotnetNiger.UI.Services.Contracts;

namespace DotnetNiger.UI.Services.Auth;

public class AuthService : IAuthService
{
    private readonly HttpClient _http;
    private readonly CustomAuthStateProvider _authProvider;

    public event Action? OnAuthStateChanged;

    public AuthService(HttpClient http, CustomAuthStateProvider authProvider)
    {
        _http = http;
        _authProvider = authProvider;
    }

    public async Task<ApiSuccessResponse<AuthDto>> LoginAsync(LoginRequest request)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", request);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await TryReadErrorMessageAsync(response.Content);

                return new ApiSuccessResponse<AuthDto>
                {
                    Success = false,
                    Message = !string.IsNullOrWhiteSpace(errorMessage)
                        ? errorMessage
                        : $"Connexion impossible (HTTP {(int)response.StatusCode})."
                };
            }

            var result = await response.Content.ReadFromJsonAsync<ApiSuccessResponse<AuthDto>>()
                         ?? new ApiSuccessResponse<AuthDto> { Success = false, Message = "Erreur de connexion." };

            if (result.Success && result.Data is not null)
                await _authProvider.SaveTokensAsync(result.Data.Token.AccessToken, result.Data.Token.RefreshToken);

            return result;
        }
        catch (HttpRequestException ex)
        {
            return new()
            {
                Success = false,
                Message = ex.Message
            }; //"Impossible de joindre le serveur d'authentification. Vérifiez que l'API est démarrée et accessible."

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
            var payloadJson = Encoding.UTF8.GetString(ParseBase64WithoutPadding(segments[1]));
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

    /// <summary>
    /// Détermine le chemin de redirection après connexion basé sur les rôles de l'utilisateur.
    /// </summary>
    /// <param name="roles">Liste des rôles de l'utilisateur</param>
    /// <returns>Le chemin vers lequel rediriger l'utilisateur</returns>
    public string GetPostLoginRedirectPath(List<string>? roles)
    {
        if (roles == null || roles.Count == 0)
            return "/";

        // Vérifier les rôles par ordre de priorité
        var rolesLower = roles.Select(r => r.ToLowerInvariant()).ToList();

        if (rolesLower.Contains("superadmin"))
            return "/admin/dashboard";

        if (rolesLower.Contains("admin"))
            return "/admin/dashboard";

        if (rolesLower.Contains("moderator"))
            return "/admin/dashboard";

        // Rôles utilisateur normal
        return "/";
    }

    /// <summary>
    /// Détermine le chemin de redirection après connexion basé sur le token d'accès (obsolète).
    /// Utiliser GetPostLoginRedirectPath(List&lt;string&gt;) avec les rôles de l'utilisateur.
    /// </summary>
    [Obsolete("Utilisez GetPostLoginRedirectPath(List<string> roles) avec les rôles du UserDto")]
    public string GetPostLoginRedirectPathFromToken(string? accessToken)
    {
        var role = GetRoleFromAccessToken(accessToken);
        if (string.IsNullOrWhiteSpace(role))
            return "/";

        return role.ToLowerInvariant() switch
        {
            "admin" => "/admin/dashboard",
            "superadmin" => "/admin/dashboard",
            "moderator" => "/admin/dashboard",
            _ => "/"
        };
    }

    public async Task<ApiSuccessResponse<AuthDto>> RegisterAsync(RegisterRequest request)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/auth/register", request);
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

            var wrapped = await response.Content.ReadFromJsonAsync<ApiSuccessResponse<AuthDto>>();
            var result = wrapped ?? new ApiSuccessResponse<AuthDto>
            {
                Success = false,
                Message = "Erreur lors de l'inscription."
            };

            if (result.Success && result.Data?.Token is not null)
                await _authProvider.SaveTokensAsync(result.Data.Token.AccessToken, result.Data.Token.RefreshToken);

            return result;
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

    public async Task<ApiSuccessResponse<Guid>> RegisterStep1Async(RegisterRequest request)
    {
        var result = await RegisterAsync(request);
        if (!result.Success || result.Data?.User is null)
        {
            return new ApiSuccessResponse<Guid>
            {
                Success = false,
                Message = result.Message ?? "Erreur lors de l'étape d'inscription."
            };
        }

        return new ApiSuccessResponse<Guid>
        {
            Success = true,
            Message = result.Message ?? "Étape 1 validée.",
            Data = result.Data.User.Id
        };
    }

    public async Task LogoutAsync()
    {
        var refreshToken = await _authProvider.GetRefreshTokenAsync();

        if (!string.IsNullOrWhiteSpace(refreshToken))
        {
            _ = await _http.PostAsJsonAsync("api/auth/logout",
                new RefreshTokenRequest { RefreshToken = refreshToken });
        }

        await _authProvider.ClearTokensAsync();
    }

    /// <summary>
    /// Renouvelle l'access token depuis le refresh token stocké.
    /// Efface la session si le refresh token est invalide ou expiré.
    /// </summary>
    public async Task<AuthDto?> RefreshTokenAsync()
    {
        var refreshToken = await _authProvider.GetRefreshTokenAsync();
        if (string.IsNullOrWhiteSpace(refreshToken))
            return null;

        var response = await _http.PostAsJsonAsync("api/auth/refresh",
            new RefreshTokenRequest { RefreshToken = refreshToken });

        if (!response.IsSuccessStatusCode)
        {
            await _authProvider.ClearTokensAsync();
            return null;
        }

        var wrapped = await response.Content.ReadFromJsonAsync<ApiSuccessResponse<AuthDto>>();
        var result = wrapped?.Data;
        if (result?.Token is not null)
            await _authProvider.SaveTokensAsync(result.Token.AccessToken, result.Token.RefreshToken);

        return result;
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

        return string.Equals(role, "admin", StringComparison.OrdinalIgnoreCase)
               || string.Equals(role, "superadmin", StringComparison.OrdinalIgnoreCase)
               || string.Equals(role, "moderator", StringComparison.OrdinalIgnoreCase);
    }

    public Task<string?> GetAccessTokenAsync()
        => _authProvider.GetAccessTokenAsync();

    public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/auth/forgot-password", request);
        return response.IsSuccessStatusCode;
    }

    public async Task<ApiSuccessResponse<object>> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/auth/reset-password", request);
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
        var response = await _http.PostAsJsonAsync("api/auth/request-email-verification", request);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> VerifyEmailAsync(VerifyEmailRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/auth/verify-email", request);
        return response.IsSuccessStatusCode;
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

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        base64 = base64.Replace('-', '+').Replace('_', '/');
        return (base64.Length % 4) switch
        {
            2 => Convert.FromBase64String(base64 + "=="),
            3 => Convert.FromBase64String(base64 + "="),
            _ => Convert.FromBase64String(base64),
        };
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
    {
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var kvs = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonBytes)!;

        return kvs.SelectMany(kv =>
        {
            if (kv.Key is "roles" or "role")
            {
                if (kv.Value.ValueKind == JsonValueKind.Array)
                    return kv.Value.EnumerateArray().Select(r => new Claim(ClaimTypes.Role, r.GetString()!));
                return new[] { new Claim(ClaimTypes.Role, kv.Value.GetString()!) };
            }

            return new[] { new Claim(kv.Key, kv.Value.ToString()) };
        });
    }
}

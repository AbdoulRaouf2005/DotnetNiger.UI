using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Models.Responses;
using DotnetNiger.UI.Services.Contracts;

namespace DotnetNiger.UI.Services.Auth;

public class AuthService : IAuthService
{
    private readonly HttpClient _http;
    private readonly CustomAuthStateProvider _authProvider;
    private readonly TokenStorageService _tokenStorage;

    public AuthService(HttpClient http, CustomAuthStateProvider authProvider, TokenStorageService tokenStorage)
    {
        _http = http;
        _authProvider = authProvider;
        _tokenStorage = tokenStorage;
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
                ["scope"] = "openid profile email roles offline_access"
            };

            var response = await _http.PostAsync("connect/token", new FormUrlEncodedContent(formData));

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
            if (authDto is null)
                return new ApiSuccessResponse<AuthDto> { Success = false, Message = error ?? "Erreur de connexion." };

            // Store tokens in localStorage pour Bearer header + httpOnly cookies via Gateway
            await _tokenStorage.SaveTokensAsync(authDto.Token.AccessToken, authDto.Token.RefreshToken);
            await StoreTokensAsync(authDto.Token.AccessToken, authDto.Token.RefreshToken);

            _authProvider.SetAuthenticatedFromUser(authDto.User);

            return new ApiSuccessResponse<AuthDto> { Success = true, Data = authDto };
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

    private async Task StoreTokensAsync(string accessToken, string? refreshToken)
    {
        try
        {
            var payload = new Dictionary<string, string?>
            {
                ["accessToken"] = accessToken,
                ["refreshToken"] = refreshToken
            };
            await _http.PostAsJsonAsync("api/auth/tokens", payload);
        }
        catch
        {
            // Non fatal: the session endpoint will still work on first call
        }
    }

    public async Task LogoutAsync()
    {
        try
        {
            await _http.DeleteAsync("api/auth/tokens");
        }
        catch
        {
            // Proceed with local state cleanup even if server call fails
        }

        await _tokenStorage.ClearAsync();
        _authProvider.NotifyStateChanged();
    }

    public async Task<UserDto?> GetCurrentUserAsync()
    {
        var state = await _authProvider.GetAuthenticationStateAsync();
        if (!state.User.Identity?.IsAuthenticated ?? true)
            return null;

        var claims = state.User.Claims.ToList();
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
    {
        var state = await _authProvider.GetAuthenticationStateAsync();
        return state.User.Identity?.IsAuthenticated ?? false;
    }

    public async Task<bool> IsAdminAsync()
    {
        var state = await _authProvider.GetAuthenticationStateAsync();
        if (!state.User.Identity?.IsAuthenticated ?? true)
            return false;

        var roles = state.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        return roles.Any(r => r.Equals("admin", StringComparison.OrdinalIgnoreCase)
                           || r.Equals("superadmin", StringComparison.OrdinalIgnoreCase)
                           || r.Equals("moderator", StringComparison.OrdinalIgnoreCase));
    }

    public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/auth/forgot-password", request);
        return response.IsSuccessStatusCode;
    }

    public async Task<ApiSuccessResponse<object>> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var resetPayload = new { email = request.Email, token = request.Token, password = request.NewPassword };
        var response = await _http.PostAsJsonAsync("api/auth/reset-password", resetPayload);
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

            var response = await _http.PostAsJsonAsync("api/auth/register", registerPayload);
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

    public async Task<ApiSuccessResponse<Guid>> RegisterStep1Async(RegisterRequest request)
    {
        var result = await RegisterAsync(request);
        if (!result.Success)
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
            Data = result.Data?.User?.Id ?? Guid.Empty
        };
    }

    public async Task<ApiSuccessResponse<AuthDto>> CompleteExternalLoginAsync(string ticket)
    {
        try
        {
            var formData = new Dictionary<string, string>
            {
                ["grant_type"] = "external_login",
                ["ticket"] = ticket,
                ["client_id"] = "web-ui",
                ["scope"] = "openid profile email roles offline_access"
            };

            var response = await _http.PostAsync("connect/token", new FormUrlEncodedContent(formData));

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
            if (authDto is null)
                return new ApiSuccessResponse<AuthDto> { Success = false, Message = error ?? "Erreur de connexion externe." };

            await _tokenStorage.SaveTokensAsync(authDto.Token.AccessToken, authDto.Token.RefreshToken);
            await StoreTokensAsync(authDto.Token.AccessToken, authDto.Token.RefreshToken);
            _authProvider.SetAuthenticatedFromUser(authDto.User);

            return new ApiSuccessResponse<AuthDto> { Success = true, Data = authDto };
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

    public string GetPostLoginRedirectPath(List<string>? roles)
    {
        if (roles == null || roles.Count == 0)
            return "/";

        var rolesLower = roles.Select(r => r.ToLowerInvariant()).ToList();

        if (rolesLower.Contains("superadmin"))
            return "/admin";

        if (rolesLower.Contains("admin"))
            return "/admin";

        if (rolesLower.Contains("moderator"))
            return "/admin";

        return "/";
    }

    private static async Task<(AuthDto?, string?)> ParseTokenResponseAsync(HttpResponseMessage response)
    {
        try
        {
            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (!root.TryGetProperty("access_token", out var atProp) || atProp.GetString() is not { } accessToken)
                return (null, "access_token manquant dans la réponse");

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

    internal static byte[] ParseBase64WithoutPadding(string base64)
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

    internal static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var segments = jwt.Split('.');
        if (segments.Length < 2)
            return [];

        var payload = segments[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var kvs = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonBytes);
        if (kvs is null)
            return [];

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

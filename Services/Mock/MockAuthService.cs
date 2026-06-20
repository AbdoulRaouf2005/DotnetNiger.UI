using DotnetNiger.UI.Helpers;
using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Models.Responses;
using DotnetNiger.UI.Services.Auth;
using DotnetNiger.UI.Services.Contracts;
using DotnetNiger.UI.Services.Helpers;
namespace DotnetNiger.UI.Services.Mock;

public class MockAuthService : IAuthService
{
    private static readonly List<UserDto> _users = MockDataStore.Users;

    private static Dictionary<string, string> _refreshTokens = new();

    private readonly CustomAuthStateProvider _authProvider;
    private UserDto? _currentUser;
    private TokenDto? _currentToken;
    private DateTime? _tokenExpiry;

    public MockAuthService(CustomAuthStateProvider authProvider)
    {
        _authProvider = authProvider;
    }

    #region Authentification

    public async Task<ApiSuccessResponse<AuthDto>> LoginAsync(LoginRequest request)
    {
        await Task.Delay(600);

        var user = _users.FirstOrDefault(u => 
            u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase));

        if (user == null || request.Password != "password123")
        {
            return new ApiSuccessResponse<AuthDto>
            {
                Success = false,
                Message = "Email ou mot de passe incorrect"
            };
        }

        if (!user.IsActive)
        {
            return new ApiSuccessResponse<AuthDto>
            {
                Success = false,
                Message = "Votre compte est désactivé. Veuillez contacter l'administrateur."
            };
        }

        _currentUser = user;
        _currentToken = GenerateTokenDto(user);
        _tokenExpiry = DateTime.Now.AddSeconds(_currentToken.ExpiresIn);

        user.LastLoginAt = DateTime.Now;
        
        // Stocker le refresh token
        _refreshTokens[user.Id.ToString()] = _currentToken.RefreshToken;

        await _authProvider.SaveTokensAsync(_currentToken.AccessToken, _currentToken.RefreshToken);

        return new ApiSuccessResponse<AuthDto>
        {
            Success = true,
            Message = "Connexion réussie",
            Data = new AuthDto
            {
                User = user,
                Token = _currentToken
            }
        };
    }

    public async Task<ApiSuccessResponse<AuthDto>> CompleteExternalLoginAsync(string ticket)
    {
        await Task.Delay(600);
        return new ApiSuccessResponse<AuthDto>
        {
            Success = false,
            Message = "Le login externe n'est pas disponible en mode mock."
        };
    }

    public async Task<ApiSuccessResponse<AuthDto>> RegisterAsync(RegisterRequest request)
    {
        await Task.Delay(800);

        // Vérifier si l'email existe déjà
        if (_users.Any(u => u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase)))
        {
            return new ApiSuccessResponse<AuthDto>
            {
                Success = false,
                Message = "Cet email est déjà utilisé"
            };
        }


        // Créer un nouvel utilisateur
        var newUser = new UserDto
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            FullName = request.FullName,
            IsActive = true,
            CreatedAt = DateTime.Now,
            Roles = new List<string> { "Member" },
            Skills = new List<string>()
        };

        _users.Add(newUser);

        return new ApiSuccessResponse<AuthDto>
        {
            Success = true,
            Message = "Inscription réussie. Veuillez vérifier votre email.",
            Data = null
        };
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
            var payloadJson = System.Text.Encoding.UTF8.GetString(JwtParser.ParseBase64WithoutPadding(segments[1]));
            using var document = System.Text.Json.JsonDocument.Parse(payloadJson);
            var root = document.RootElement;

            if (root.TryGetProperty("role", out var roleElement) && roleElement.ValueKind == System.Text.Json.JsonValueKind.String)
                return roleElement.GetString();

            if (root.TryGetProperty("roles", out var rolesElement))
            {
                if (rolesElement.ValueKind == System.Text.Json.JsonValueKind.Array)
                    return rolesElement.EnumerateArray().Select(x => x.GetString()).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));

                if (rolesElement.ValueKind == System.Text.Json.JsonValueKind.String)
                    return rolesElement.GetString();
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task LogoutAsync()
    {
        await Task.Delay(200);
        
        if (_currentUser != null)
        {
            _refreshTokens.Remove(_currentUser.Id.ToString());
        }
        
        _currentUser = null;
        _currentToken = null;
        _tokenExpiry = null;
        
        return;
    }

    #endregion

    #region Gestion de compte

    public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        await Task.Delay(500);
        
        var user = _users.FirstOrDefault(u => 
            u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase));

        if (user == null)
        {
            return true;
        }

        return true;
    }

    public async Task<ApiSuccessResponse<object>> ResetPasswordAsync(ResetPasswordRequest request)
    {
        await Task.Delay(500);
        
        var user = _users.FirstOrDefault(u => 
            u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase));

        if (user == null)
        {
            return new ApiSuccessResponse<object>
            {
                Success = false,
                Message = "Email invalide",
                Data = false
            };
        }

        // Simuler la vérification du token
        if (request.Token != "valid-reset-token")
        {
            return new ApiSuccessResponse<object>
            {
                Success = false,
                Message = "Token invalide ou expiré",
                Data = false
            };
        }

        return new ApiSuccessResponse<object>
        {
            Success = true,
            Message = "Votre mot de passe a été réinitialisé avec succès.",
            Data = true
        };
    }

    public async Task<bool> RequestEmailVerificationAsync(RequestEmailVerificationRequest request)
    {
        await Task.Delay(500);
        
        var user = _users.FirstOrDefault(u => 
            u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase));

        if (user == null)
        {
            return false;
        }

        return true;
    }

    public async Task<bool> VerifyEmailAsync(VerifyEmailRequest request)
    {
        await Task.Delay(500);
        
        var user = _users.FirstOrDefault(u => 
            u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase));

        if (user == null)
        {
            return false;
        }

        // Simuler la vérification du token
        if (request.Code != "valid-verification-code")
        {
            return false;
        }

        return true;
    }

    #endregion

    #region Refresh Token

    public async Task<AuthDto?> RefreshTokenAsync()
    {
        await Task.Delay(500);

        var user = _currentUser;
        if (user is null || _currentToken is null || _tokenExpiry <= DateTime.Now)
            return null;

        var newToken = GenerateTokenDto(user);
        _currentToken = newToken;
        _tokenExpiry = DateTime.Now.AddSeconds(newToken.ExpiresIn);
        _refreshTokens[user.Id.ToString()] = newToken.RefreshToken;

        return new AuthDto { User = user, Token = newToken };
    }

    #endregion

    #region État utilisateur

    public async Task<UserDto?> GetCurrentUserAsync()
    {
        await Task.Delay(100);
        return _currentUser;
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        await Task.Delay(50);
        return _currentUser != null && _tokenExpiry > DateTime.Now;
    }

    public async Task<bool> IsAdminAsync()
    {
        await Task.Delay(50);
        return _currentUser?.Roles.Contains("Admin") ?? false;
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        await Task.Delay(50);
        
        if (_currentToken == null || _tokenExpiry <= DateTime.Now)
        {
            return null;
        }
        
        return _currentToken.AccessToken;
    }

    #endregion

    #region Méthodes privées

    private TokenDto GenerateTokenDto(UserDto user)
    {
        // Simuler un JWT (token mocké)
        var mockToken = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(
            $"{{\"sub\":\"{user.Id}\",\"email\":\"{user.Email}\",\"role\":\"{user.Roles.FirstOrDefault()}\",\"exp\":{DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds()}}}"));
        
        return new TokenDto
        {
            AccessToken = $"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.{mockToken}.mock-signature",
            RefreshToken = GenerateRefreshToken(),
            ExpiresIn = 3600, // 1 heure en secondes
            TokenType = "Bearer"
        };
    }

    private string GenerateRefreshToken()
    {
        return $"refresh-{Guid.NewGuid()}-{DateTime.Now.Ticks}";
    }

    #endregion
}
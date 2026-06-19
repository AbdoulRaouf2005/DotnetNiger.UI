// Services/Mocks/MockAuthService.cs
using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Models.Responses;
using DotnetNiger.UI.Services.Contracts;
namespace DotnetNiger.UI.Services.Mock;

public class MockAuthService : IAuthService
{
    private static readonly List<UserDto> _users = new()
    {
        new UserDto
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Username = "admin",
            Email = "admin@dotnet.niger",
            FullName = "Ali Mahamane",
            Country = "Niger",
            City = "Niamey",
            IsActive = true,
            CreatedAt = DateTime.Now.AddMonths(-6),
            LastLoginAt = DateTime.Now.AddDays(-1),
            Roles = new List<string> { "Admin", "Member" },
            Skills = new List<string> { "C#", "ASP.NET Core", "Blazor", "Azure" }
        },
        new UserDto
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Username = "fatima",
            Email = "fatima@dotnet.niger",
            FullName = "Fatima Oumar",
            Country = "Niger",
            City = "Niamey",
            IsActive = true,
            CreatedAt = DateTime.Now.AddMonths(-3),
            LastLoginAt = DateTime.Now.AddDays(-5),
            Roles = new List<string> { "Member" },
            Skills = new List<string> { "C#", "ASP.NET Core", "Entity Framework" }
        },
        new UserDto
        {
            Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            Username = "moussa",
            Email = "moussa@dotnet.niger",
            FullName = "Moussa Issa",
            Country = "Niger",
            City = "Maradi",
            IsActive = true,
            CreatedAt = DateTime.Now.AddMonths(-1),
            Roles = new List<string> { "Member" },
            Skills = new List<string> { "C#", "Blazor" }
        }
    };

    private static Dictionary<string, string> _refreshTokens = new();
    private static List<ForgotPasswordRequest> _passwordResetRequests = new();
    private static List<RequestEmailVerificationRequest> _emailVerificationRequests = new();

    private UserDto? _currentUser;
    private TokenDto? _currentToken;
    private DateTime? _tokenExpiry;

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

    public string GetPostLoginRedirectPath(List<string>? roles)
    {
        if (roles == null || roles.Count == 0)
            return "/";

        var rolesLower = roles.Select(r => r.ToLowerInvariant()).ToList();
        if (rolesLower.Contains("superadmin") || rolesLower.Contains("admin") || rolesLower.Contains("moderator"))
            return "/admin";

        return "/";
    }

    public async Task<ApiSuccessResponse<Guid>> RegisterStep1Async(RegisterRequest request)
    {
        var result = await RegisterAsync(request);
        if (!result.Success)
        {
            return new ApiSuccessResponse<Guid>
            {
                Success = false,
                Message = result.Message ?? "Erreur lors de l'inscription.",
                Data = Guid.Empty
            };
        }

        var createdUser = _users.FirstOrDefault(u => u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase));
        return new ApiSuccessResponse<Guid>
        {
            Success = true,
            Message = "Étape 1 validée.",
            Data = createdUser?.Id ?? Guid.Empty
        };
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

        _passwordResetRequests.Add(request);

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

        _emailVerificationRequests.Add(request);

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
        return _currentUser?.Roles.Contains("Admin", StringComparer.OrdinalIgnoreCase) ?? false;
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

    #endregion
}
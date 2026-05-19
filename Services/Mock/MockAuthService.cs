using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Models.Responses;
using DotnetNiger.UI.Services.Contracts;

namespace DotnetNiger.UI.Services.Mock;

public class MockAuthService : IAuthService
{
    public async Task<ApiSuccessResponse<AuthDto>> LoginAsync(LoginRequest request)
    {
        await Task.Delay(500);

        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return new ApiSuccessResponse<AuthDto>
            {
                Success = false,
                Message = "Email et mot de passe requis."
            };
        }

        return new ApiSuccessResponse<AuthDto>
        {
            Success = true,
            Message = "Connexion réussie.",
            Data = BuildAuthDto(request.Email, "Membre .NET Niger")
        };
    }

    public string? GetRoleFromAccessToken(string? accessToken)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
            return null;

        return accessToken.Contains("admin", StringComparison.OrdinalIgnoreCase) ? "admin" : "member";
    }

    public string GetPostLoginRedirectPath(List<string>? roles)
    {
        if (roles is null || roles.Count == 0)
            return "/";

        var normalizedRoles = roles.Select(r => r.ToLowerInvariant()).ToList();

        if (normalizedRoles.Contains("superadmin") || normalizedRoles.Contains("admin") || normalizedRoles.Contains("moderator"))
            return "/admin/dashboard";

        return "/";
    }

    public string GetPostLoginRedirectPathFromToken(string? accessToken)
    {
        var role = GetRoleFromAccessToken(accessToken);
        if (string.IsNullOrWhiteSpace(role))
            return "/";

        return role.Equals("admin", StringComparison.OrdinalIgnoreCase)
            ? "/admin/dashboard"
            : "/";
    }

    public async Task<ApiSuccessResponse<AuthDto>> RegisterAsync(RegisterRequest request)
    {
        await Task.Delay(700);

        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password) || string.IsNullOrWhiteSpace(request.FullName))
        {
            return new ApiSuccessResponse<AuthDto>
            {
                Success = false,
                Message = "Veuillez renseigner les informations d'inscription."
            };
        }

        return new ApiSuccessResponse<AuthDto>
        {
            Success = true,
            Message = "Compte créé avec succès.",
            Data = BuildAuthDto(request.Email, request.FullName)
        };
    }

    public async Task<ApiSuccessResponse<Guid>> RegisterStep1Async(RegisterRequest request)
    {
        var result = await RegisterAsync(request);

        if (!result.Success || result.Data?.User is null)
        {
            return new ApiSuccessResponse<Guid>
            {
                Success = false,
                Message = result.Message ?? "Échec de l'étape 1."
            };
        }

        return new ApiSuccessResponse<Guid>
        {
            Success = true,
            Message = "Étape 1 validée.",
            Data = result.Data.User.Id
        };
    }

    public Task LogoutAsync() => Task.CompletedTask;

    public async Task<AuthDto?> RefreshTokenAsync()
    {
        await Task.Delay(200);
        return BuildAuthDto("mock@dotnetniger.org", "Membre .NET Niger");
    }

    public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        await Task.Delay(300);
        return !string.IsNullOrWhiteSpace(request.Email);
    }

    public async Task<ApiSuccessResponse<object>> ResetPasswordAsync(ResetPasswordRequest request)
    {
        await Task.Delay(300);

        var isValid = !string.IsNullOrWhiteSpace(request.Token) &&
                      !string.IsNullOrWhiteSpace(request.NewPassword) &&
                      request.NewPassword.Length >= 8;

        return new ApiSuccessResponse<object>
        {
            Success = isValid,
            Message = isValid
                ? "Mot de passe réinitialisé avec succès."
                : "Données de réinitialisation invalides."
        };
    }

    public async Task<bool> RequestEmailVerificationAsync(RequestEmailVerificationRequest request)
    {
        await Task.Delay(250);
        return !string.IsNullOrWhiteSpace(request.Email);
    }

    public async Task<bool> VerifyEmailAsync(VerifyEmailRequest request)
    {
        await Task.Delay(250);
        return !string.IsNullOrWhiteSpace(request.Token);
    }

    private static AuthDto BuildAuthDto(string email, string fullName)
    {
        return new AuthDto
        {
            User = new UserDto
            {
                Id = Guid.NewGuid(),
                Email = email,
                FullName = fullName,
                Username = email,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                Roles = new List<string> { "Member" }
            },
            Token = new TokenDto
            {
                AccessToken = "mock-access-token",
                RefreshToken = "mock-refresh-token",
                ExpiresIn = 3600,
                TokenType = "Bearer"
            }
        };
    }
}

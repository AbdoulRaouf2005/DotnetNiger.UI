// Services/IAuthService.cs
using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Models.Responses;

namespace DotnetNiger.UI.Services.Contracts;

public interface IAuthService
{
    event Action? OnAuthStateChanged;

    string? GetRoleFromAccessToken(string? accessToken);
    string GetPostLoginRedirectPath(List<string>? roles);
    string GetPostLoginRedirectPathFromToken(string? accessToken);
    
    // Authentification
    Task<ApiSuccessResponse<AuthDto>> LoginAsync(LoginRequest request);
    Task<ApiSuccessResponse<AuthDto>> RegisterAsync(RegisterRequest request);
    Task<ApiSuccessResponse<Guid>> RegisterStep1Async(RegisterRequest request);
    Task LogoutAsync();
    
    // Gestion de compte
    Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request);
    Task<ApiSuccessResponse<object>> ResetPasswordAsync(ResetPasswordRequest request);
    Task<bool> RequestEmailVerificationAsync(RequestEmailVerificationRequest request);
    Task<bool> VerifyEmailAsync(VerifyEmailRequest request);
    
    // Refresh token
    Task<AuthDto?> RefreshTokenAsync();
    
    // État utilisateur
    Task<UserDto?> GetCurrentUserAsync();
    Task<bool> IsAuthenticatedAsync();
    Task<bool> IsAdminAsync();
    Task<string?> GetAccessTokenAsync();
}
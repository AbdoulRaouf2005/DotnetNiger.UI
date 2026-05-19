using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Models.Responses;

namespace DotnetNiger.UI.Services.Contracts;

public interface IAuthService
{
    Task<ApiSuccessResponse<AuthDto>> LoginAsync(LoginRequest request);
    string? GetRoleFromAccessToken(string? accessToken);
    string GetPostLoginRedirectPath(List<string>? roles);
    string GetPostLoginRedirectPathFromToken(string? accessToken);
    Task<ApiSuccessResponse<AuthDto>> RegisterAsync(RegisterRequest request);
    Task<ApiSuccessResponse<Guid>> RegisterStep1Async(RegisterRequest request);
    Task LogoutAsync();
    Task<AuthDto?> RefreshTokenAsync();
    Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request);
    Task<ApiSuccessResponse<object>> ResetPasswordAsync(ResetPasswordRequest request);
    Task<bool> RequestEmailVerificationAsync(RequestEmailVerificationRequest request);
    Task<bool> VerifyEmailAsync(VerifyEmailRequest request);
}

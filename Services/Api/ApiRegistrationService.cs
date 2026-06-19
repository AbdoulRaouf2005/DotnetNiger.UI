using System.Net.Http.Json;
using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Models.Responses;
using DotnetNiger.UI.Services.Contracts;

namespace DotnetNiger.UI.Services.Api;

public class ApiRegistrationService : IRegistrationService
{
    private readonly HttpClient _http;
    private const string CertificateBase = "api/profile/certificates";

    public ApiRegistrationService(HttpClient http) => _http = http;

    public Task<ApiSuccessResponse<Guid>> SubmitStep1Async(RegisterRequest request)
    {
        // L'étape 1 est gérée via la redirection externe vers Identity Server (/Account/Register).
        throw new NotSupportedException("L'étape 1 est gérée par Identity Server via redirection externe.");
    }

    public async Task<ApiSuccessResponse<CertificateStatusDto>> SubmitStep2Async(CertificateSubmissionDto request)
    {
        var response = await _http.PostAsJsonAsync(CertificateBase, request);
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            return new ApiSuccessResponse<CertificateStatusDto>
            {
                Success = false,
                Message = string.IsNullOrWhiteSpace(errorBody) ? "Erreur lors de la soumission du certificat." : errorBody
            };
        }
        return await response.Content.ReadFromJsonAsync<ApiSuccessResponse<CertificateStatusDto>>()
            ?? new ApiSuccessResponse<CertificateStatusDto> { Success = true };
    }
}

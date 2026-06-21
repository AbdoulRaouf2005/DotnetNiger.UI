using System.Net.Http.Json;
using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Models.Responses;
using DotnetNiger.UI.Services.Contracts;

namespace DotnetNiger.UI.Services.Api;

public class ApiRegistrationService : ApiServiceBase, IRegistrationService
{
    public ApiRegistrationService(HttpClient http) : base(http) { }

    public Task<ApiSuccessResponse<Guid>> SubmitStep1Async(RegisterRequest request)
    {
        throw new NotSupportedException("L'étape 1 est gérée par Identity Server via redirection externe.");
    }

    public async Task<ApiSuccessResponse<CertificateStatusDto>> SubmitStep2Async(CertificateSubmissionDto request)
    {
        var response = await Http.PostAsJsonAsync(ApiEndpoints.Certificates, request);
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            return new ApiSuccessResponse<CertificateStatusDto>
            {
                Success = false,
                Message = string.IsNullOrWhiteSpace(errorBody) ? "Erreur lors de la soumission du certificat." : errorBody
            };
        }

        var result = await ApiResponseReader.ReadAsync<CertificateStatusDto>(response);
        return result is not null
            ? new ApiSuccessResponse<CertificateStatusDto> { Success = true, Data = result }
            : new ApiSuccessResponse<CertificateStatusDto> { Success = true };
    }
}

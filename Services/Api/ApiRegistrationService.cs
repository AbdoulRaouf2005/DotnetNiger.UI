using System.Net.Http.Json;
using System.Text.Json;
using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Models.Responses;
using DotnetNiger.UI.Services.Contracts;

namespace DotnetNiger.UI.Services.Api;

public class ApiRegistrationService : IRegistrationService
{
    private readonly HttpClient _http;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    public ApiRegistrationService(HttpClient http)
    {
        _http = http;
    }

    public Task<ApiSuccessResponse<Guid>> SubmitStep1Async(RegisterRequest request)
    {
        throw new NotSupportedException("L'étape 1 est gérée par redirection vers le fournisseur d'identité.");
    }

    public async Task<ApiSuccessResponse<CertificateStatusDto>> SubmitStep2Async(CertificateSubmissionDto request)
    {
        var response = await _http.PostAsJsonAsync("api/v1/profile/certificates", request);
        var json = await response.Content.ReadAsStringAsync();

        try
        {
            var wrapped = JsonSerializer.Deserialize<ApiSuccessResponse<CertificateStatusDto>>(json, JsonOptions);
            return wrapped ?? new ApiSuccessResponse<CertificateStatusDto>
            {
                Success = false,
                Message = "Erreur inattendue."
            };
        }
        catch
        {
            return new ApiSuccessResponse<CertificateStatusDto>
            {
                Success = false,
                Message = "Erreur de communication avec le serveur."
            };
        }
    }
}

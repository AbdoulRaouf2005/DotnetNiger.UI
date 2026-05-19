using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Models.Responses;

namespace DotnetNiger.UI.Services.Contracts;

public interface IRegistrationService
{
    Task<ApiSuccessResponse<Guid>> SubmitStep1Async(RegisterRequest request);
    Task<ApiSuccessResponse<CertificateStatusDto>> SubmitStep2Async(CertificateSubmissionDto request);
}

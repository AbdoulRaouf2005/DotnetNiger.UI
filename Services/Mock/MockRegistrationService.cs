using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Models.Responses;
using DotnetNiger.UI.Services.Contracts;

namespace DotnetNiger.UI.Services.Mock;

public class MockRegistrationService : IRegistrationService
{
    public async Task<ApiSuccessResponse<Guid>> SubmitStep1Async(RegisterRequest request)
    {
        await Task.Delay(600);

        if (string.IsNullOrWhiteSpace(request.FullName) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return new ApiSuccessResponse<Guid>
            {
                Success = false,
                Message = "Veuillez renseigner toutes les informations requises."
            };
        }

        return new ApiSuccessResponse<Guid>
        {
            Success = true,
            Message = "Étape 1 validée.",
            Data = Guid.NewGuid()
        };
    }

    public async Task<ApiSuccessResponse<CertificateStatusDto>> SubmitStep2Async(CertificateSubmissionDto request)
    {
        await Task.Delay(800);

        if (request.UserId == Guid.Empty)
        {
            return new ApiSuccessResponse<CertificateStatusDto>
            {
                Success = false,
                Message = "Utilisateur introuvable. Veuillez recommencer l'inscription."
            };
        }

        if (!Uri.TryCreate(request.CertificateUrl, UriKind.Absolute, out _))
        {
            return new ApiSuccessResponse<CertificateStatusDto>
            {
                Success = false,
                Message = "URL de certification invalide."
            };
        }

        if (string.IsNullOrWhiteSpace(request.CertificateType))
        {
            return new ApiSuccessResponse<CertificateStatusDto>
            {
                Success = false,
                Message = "Veuillez sélectionner un type de certificat."
            };
        }

        return new ApiSuccessResponse<CertificateStatusDto>
        {
            Success = true,
            Message = "Certification soumise avec succès.",
            Data = new CertificateStatusDto
            {
                Status = "Pending",
                SubmissionDate = DateTime.UtcNow,
                EstimatedWaitTime = "24-48 heures",
                SupportEmail = "support@dotnetniger.org"
            }
        };
    }
}

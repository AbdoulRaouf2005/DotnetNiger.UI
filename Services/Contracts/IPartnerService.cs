using DotnetNiger.UI.Models.Responses;

namespace DotnetNiger.UI.Services.Contracts;

public interface IPartnerService
{
    Task<List<PartnerResponse>> GetAllActiveAsync(string? partnerType);
    Task<PartnerResponse?> GetByIdAsync(Guid id);
}

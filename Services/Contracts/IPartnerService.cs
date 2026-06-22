using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Models.Responses;

namespace DotnetNiger.UI.Services.Contracts;

public interface IPartnerService
{
    Task<List<PartnerResponse>> GetAllActiveAsync(string? partnerType);
    Task<List<PartnerResponse>> GetAllAsync();
    Task<PartnerResponse?> GetByIdAsync(Guid id);
    Task<PartnerResponse?> CreateAsync(CreatePartnerRequest request);
    Task<PartnerResponse?> UpdateAsync(Guid id, UpdatePartnerRequest request);
    Task<bool> DeleteAsync(Guid id);
}

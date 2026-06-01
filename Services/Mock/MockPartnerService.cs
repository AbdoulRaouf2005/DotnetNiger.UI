using DotnetNiger.UI.Models.Responses;
using DotnetNiger.UI.Services.Contracts;

namespace DotnetNiger.UI.Services.Mock;

public class MockPartnerService : IPartnerService
{
    private readonly List<PartnerResponse> _partners = new()
    {
        new PartnerResponse
        {
            Id = Guid.NewGuid(), Name = "Université Abdou Moumouni", Slug = "universite-abdou-moumouni",
            Description = "Plateforme innovante offrant des espaces de travail collaboratifs pour les startups et développeurs.",
            PartnerType = "education", SortOrder = 1, IsActive = true,
            CreatedAt = DateTime.UtcNow.AddMonths(-12)
        },
        new PartnerResponse
        {
            Id = Guid.NewGuid(), Name = "Orange Niger", Slug = "orange-niger",
            Description = "Opérateur télécom majeur soutenant l'innovation numérique au Niger.",
            WebsiteUrl = "https://www.orange.ne", PartnerType = "sponsor", SortOrder = 2, IsActive = true,
            CreatedAt = DateTime.UtcNow.AddMonths(-8)
        },
        new PartnerResponse
        {
            Id = Guid.NewGuid(), Name = "Microsoft Learn", Slug = "microsoft-learn",
            Description = "Programme de formation et de certification pour les développeurs .NET.",
            WebsiteUrl = "https://learn.microsoft.com", PartnerType = "education", SortOrder = 3, IsActive = true,
            CreatedAt = DateTime.UtcNow.AddMonths(-6)
        }
    };

    public Task<List<PartnerResponse>> GetAllActiveAsync(string? partnerType)
    {
        var result = _partners.Where(p => p.IsActive).AsEnumerable();
        if (!string.IsNullOrWhiteSpace(partnerType))
            result = result.Where(p => p.PartnerType.Equals(partnerType, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(result.OrderBy(p => p.SortOrder).ToList());
    }

    public Task<PartnerResponse?> GetByIdAsync(Guid id) =>
        Task.FromResult(_partners.FirstOrDefault(p => p.Id == id));
}

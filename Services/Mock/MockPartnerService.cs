using DotnetNiger.UI.Models.Requests;
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

    public async Task<List<PartnerResponse>> GetAllActiveAsync(string? partnerType)
    {
        await Task.Delay(800);
        var result = _partners.Where(p => p.IsActive).AsEnumerable();
        if (!string.IsNullOrWhiteSpace(partnerType))
            result = result.Where(p => p.PartnerType.Equals(partnerType, StringComparison.OrdinalIgnoreCase));
        return result.OrderBy(p => p.SortOrder).ToList();
    }

    public async Task<List<PartnerResponse>> GetAllAsync()
    {
        await Task.Delay(500);
        return _partners.OrderBy(p => p.SortOrder).ToList();
    }

    public async Task<PartnerResponse?> GetByIdAsync(Guid id)
    {
        await Task.Delay(500);
        return _partners.FirstOrDefault(p => p.Id == id);
    }

    public async Task<PartnerResponse?> CreateAsync(CreatePartnerRequest request)
    {
        await Task.Delay(500);
        var partner = new PartnerResponse
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Slug = request.Name.ToLowerInvariant().Replace(' ', '-'),
            Description = request.Description,
            LogoUrl = request.LogoUrl,
            WebsiteUrl = request.WebsiteUrl,
            PartnerType = request.PartnerType,
            SortOrder = request.SortOrder,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow
        };
        _partners.Add(partner);
        return partner;
    }

    public async Task<PartnerResponse?> UpdateAsync(Guid id, UpdatePartnerRequest request)
    {
        await Task.Delay(500);
        var existing = _partners.FirstOrDefault(p => p.Id == id);
        if (existing is null) return null;

        existing.Name = request.Name;
        existing.Description = request.Description;
        existing.LogoUrl = request.LogoUrl;
        existing.WebsiteUrl = request.WebsiteUrl;
        existing.PartnerType = request.PartnerType;
        existing.SortOrder = request.SortOrder;
        existing.IsActive = request.IsActive;
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        await Task.Delay(500);
        var existing = _partners.FirstOrDefault(p => p.Id == id);
        if (existing is null) return false;
        return _partners.Remove(existing);
    }
}

using DotnetNiger.UI.Models.Responses;
using DotnetNiger.UI.Services.Contracts;

namespace DotnetNiger.UI.Services.Mock;

public class MockMemberDirectoryService : IMemberDirectoryService
{
    private readonly List<MemberDirectoryResponse> _members = new()
    {
        new MemberDirectoryResponse
        {
            Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            FullName = "Amadou Diallo", Bio = "Senior .NET Developer & Mentor",
            Country = "Niger", City = "Niamey", CreatedAt = DateTime.UtcNow.AddMonths(-6)
        },
        new MemberDirectoryResponse
        {
            Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
            FullName = "Fatima Ibrahim", Bio = "Fullstack Cloud Engineer",
            Country = "Niger", City = "Maradi", CreatedAt = DateTime.UtcNow.AddMonths(-4)
        },
        new MemberDirectoryResponse
        {
            Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
            FullName = "Moussa Harouna", Bio = "Backend Developer & Unity Enthusiast",
            Country = "Niger", City = "Zinder", CreatedAt = DateTime.UtcNow.AddMonths(-2)
        }
    };

    public async Task<PaginatedDto<MemberDirectoryResponse>> GetAllAsync(string? query, string? country, int page = 1, int pageSize = 10)
    {
        await Task.Delay(800);
        var filtered = _members.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(query))
            filtered = filtered.Where(m =>
                m.FullName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                m.Bio.Contains(query, StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrWhiteSpace(country))
            filtered = filtered.Where(m => m.Country.Equals(country, StringComparison.OrdinalIgnoreCase));

        var items = filtered.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return new PaginatedDto<MemberDirectoryResponse>
        {
            Items = items, TotalCount = filtered.Count(), Page = page, PageSize = pageSize
        };
    }

    public async Task<MemberDirectoryResponse?> GetByIdAsync(Guid id)
    {
        await Task.Delay(800);
        return _members.FirstOrDefault(m => m.Id == id);
    }
}

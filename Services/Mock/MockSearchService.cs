using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Models.Responses;
using DotnetNiger.UI.Services.Contracts;

namespace DotnetNiger.UI.Services.Mock;

public class MockSearchService : ISearchService
{
    private readonly List<SearchResultDto> _results = new()
    {
        new SearchResultDto
        {
            Type = "Post", Id = Guid.NewGuid(), Title = "Bienvenue dans la communauté .NET Niger",
            Slug = "bienvenue-communaute-dotnet-niger", Excerpt = "Découvrez notre communauté de développeurs.",
            CreatedAt = DateTime.UtcNow.AddMonths(-1)
        },
        new SearchResultDto
        {
            Type = "Event", Id = Guid.NewGuid(), Title = "Meetup .NET Niger — Juin 2026",
            Slug = "meetup-juin-2026", Description = "Rencontre mensuelle des développeurs .NET.",
            StartDateTime = DateTime.UtcNow.AddDays(30), CreatedAt = DateTime.UtcNow.AddDays(-5)
        },
        new SearchResultDto
        {
            Type = "Resource", Id = Guid.NewGuid(), Title = "Introduction à ASP.NET Core",
            Slug = "introduction-aspnet-core", Excerpt = "Guide complet pour débuter avec ASP.NET Core.",
            CreatedAt = DateTime.UtcNow.AddMonths(-2)
        }
    };

    public Task<PaginatedDto<SearchResultDto>> SearchAsync(SearchQueryRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
            return Task.FromResult(new PaginatedDto<SearchResultDto>());

        var filtered = _results.Where(r =>
            r.Title.Contains(request.Query, StringComparison.OrdinalIgnoreCase) ||
            (r.Excerpt?.Contains(request.Query, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (r.Description?.Contains(request.Query, StringComparison.OrdinalIgnoreCase) ?? false));

        if (!string.IsNullOrWhiteSpace(request.Type))
            filtered = filtered.Where(r => r.Type.Equals(request.Type, StringComparison.OrdinalIgnoreCase));

        var items = filtered.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList();
        return Task.FromResult(new PaginatedDto<SearchResultDto>
        {
            Items = items, TotalCount = filtered.Count(), Page = request.Page, PageSize = request.PageSize
        });
    }
}

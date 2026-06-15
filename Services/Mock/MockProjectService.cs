using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Models.Responses;
using DotnetNiger.UI.Services.Contracts;

namespace DotnetNiger.UI.Services.Mock;

public class MockProjectService : IProjectService
{
    private readonly List<ProjectResponse> _projects = new()
    {
        new ProjectResponse
        {
            Id = Guid.NewGuid(), Title = "DotnetNiger Web Platform", Slug = "dotnetniger-web-platform",
            Description = "Plateforme communautaire .NET Niger — blog, événements, ressources.",
            Url = "https://github.com/dotnetniger/platform", GithubUrl = "https://github.com/dotnetniger/platform",
            Technologies = "ASP.NET Core, Blazor, PostgreSQL", Status = "active",
            AuthorName = "Équipe DotnetNiger", IsFeatured = true, IsPublished = true,
            CreatedAt = DateTime.UtcNow.AddMonths(-3)
        },
        new ProjectResponse
        {
            Id = Guid.NewGuid(), Title = "Niger Open Data API", Slug = "niger-open-data-api",
            Description = "API ouverte pour les données publiques du Niger.",
            Url = "https://github.com/dotnetniger/opendata", GithubUrl = "https://github.com/dotnetniger/opendata",
            Technologies = "ASP.NET Core, Entity Framework, Swagger", Status = "active",
            AuthorName = "Amadou Diallo", IsFeatured = true, IsPublished = true,
            CreatedAt = DateTime.UtcNow.AddMonths(-6)
        }
    };

    public Task<PaginatedDto<ProjectResponse>> GetAllAsync(string? status, string? query, int page = 1, int pageSize = 10)
    {
        var filtered = _projects.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(status))
            filtered = filtered.Where(p => p.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrWhiteSpace(query))
            filtered = filtered.Where(p =>
                p.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                p.Description.Contains(query, StringComparison.OrdinalIgnoreCase));

        var list = filtered.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return Task.FromResult(new PaginatedDto<ProjectResponse>
        {
            Items = list, TotalCount = filtered.Count(), Page = page, PageSize = pageSize
        });
    }

    public Task<List<ProjectResponse>> GetFeaturedAsync() =>
        Task.FromResult(_projects.Where(p => p.IsFeatured).ToList());

    public Task<ProjectResponse?> GetByIdAsync(Guid id) =>
        Task.FromResult(_projects.FirstOrDefault(p => p.Id == id));

    public Task<ProjectResponse?> CreateAsync(CreateProjectRequest request)
    {
        var project = new ProjectResponse
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Slug = request.Title.ToLowerInvariant().Replace(" ", "-"),
            Description = request.Description,
            Url = request.Url,
            GithubUrl = request.GithubUrl,
            ImageUrl = request.ImageUrl,
            Technologies = request.Technologies,
            Status = request.Status,
            IsFeatured = request.IsFeatured,
            IsPublished = request.IsPublished,
            CreatedAt = DateTime.UtcNow,
            AuthorName = "Vous"
        };
        _projects.Add(project);
        return Task.FromResult<ProjectResponse?>(project);
    }

    public Task<ProjectResponse?> UpdateAsync(Guid id, UpdateProjectRequest request)
    {
        var existing = _projects.FirstOrDefault(p => p.Id == id);
        if (existing is null) return Task.FromResult<ProjectResponse?>(null);

        existing.Title = request.Title;
        existing.Description = request.Description;
        existing.Url = request.Url;
        existing.GithubUrl = request.GithubUrl;
        existing.ImageUrl = request.ImageUrl;
        existing.Technologies = request.Technologies;
        existing.Status = request.Status;
        existing.IsFeatured = request.IsFeatured;
        existing.IsPublished = request.IsPublished;
        existing.UpdatedAt = DateTime.UtcNow;
        return Task.FromResult<ProjectResponse?>(existing);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        var removed = _projects.RemoveAll(p => p.Id == id) > 0;
        return Task.FromResult(removed);
    }
}

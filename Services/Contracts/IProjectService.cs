using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Models.Responses;

namespace DotnetNiger.UI.Services.Contracts;

public interface IProjectService
{
    Task<PaginatedDto<ProjectResponse>> GetAllAsync(string? status, string? query, int page = 1, int pageSize = 10);
    Task<List<ProjectResponse>> GetFeaturedAsync();
    Task<ProjectResponse?> GetByIdAsync(Guid id);
    Task<ProjectResponse?> CreateAsync(CreateProjectRequest request);
    Task<ProjectResponse?> UpdateAsync(Guid id, UpdateProjectRequest request);
    Task<bool> DeleteAsync(Guid id);
}

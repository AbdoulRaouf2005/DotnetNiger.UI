using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Models.Responses;

namespace DotnetNiger.UI.Services.Contracts;

public interface IResourceService
{
    Task<List<ResourceDto>> GetAllResourcesAsync();
    Task<ResourceDto?> GetResourceByIdAsync(Guid id);
    Task<ResourceDto?> GetResourceBySlugAsync(string slug);
    Task<List<ResourceDto>> GetResourcesByTypeAsync(string resourceType);
    Task<List<ResourceDto>> GetResourcesByLevelAsync(string level);
    Task<List<ResourceDto>> SearchResourcesAsync(string query);
    Task<List<string>> GetResourceTypesAsync();
    Task<List<string>> GetLevelsAsync();
    Task<ResourceDto?> CreateResourceAsync(CreateResourceRequest request);
    Task<ResourceDto?> AddResourceAsync(AddResourceRequest request);
    Task<ResourceDto?> UpdateResourceAsync(Guid id, CreateResourceRequest request);
    Task<bool> DeleteResourceAsync(Guid id);
    Task IncrementViewCountAsync(Guid id);
}

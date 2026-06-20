using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Models.Responses;
using DotnetNiger.UI.Services.Contracts;
using System.Net.Http.Json;

namespace DotnetNiger.UI.Services.Api;

public class ApiResourceService : ApiServiceBase, IResourceService
{
    public ApiResourceService(HttpClient http) : base(http)
    {
    }

    public async Task<List<ResourceDto>> GetAllResourcesAsync()
    {
        return await GetCollectionAsync<ResourceDto>(ApiEndpoints.Resources);
    }

    public async Task<ResourceDto?> GetResourceByIdAsync(Guid id)
    {
        var response = await Http.GetAsync($"{ApiEndpoints.Resources}/{id}");
        if (!response.IsSuccessStatusCode)
            return null;

        return await ApiResponseReader.ReadAsync<ResourceDto>(response);
    }

    public async Task<ResourceDto?> GetResourceBySlugAsync(string slug)
    {
        var response = await Http.GetAsync($"{ApiEndpoints.Resources}/{slug}");
        if (!response.IsSuccessStatusCode)
            return null;

        return await ApiResponseReader.ReadAsync<ResourceDto>(response);
    }

    public async Task<List<ResourceDto>> GetResourcesByTypeAsync(string resourceType)
    {
        var resources = await GetCollectionAsync<ResourceDto>(ApiEndpoints.Resources, new Dictionary<string, string?>
        {
            ["resourceType"] = resourceType
        });

        return resources.Where(r => r.ResourceType.Equals(resourceType, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public async Task<List<ResourceDto>> GetResourcesByLevelAsync(string level)
    {
        var resources = await GetCollectionAsync<ResourceDto>(ApiEndpoints.Resources, new Dictionary<string, string?>
        {
            ["level"] = level
        });

        return resources.Where(r => r.Level.Equals(level, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public async Task<List<ResourceDto>> SearchResourcesAsync(string query)
    {
        return await GetCollectionAsync<ResourceDto>(ApiEndpoints.Resources, new Dictionary<string, string?>
        {
            ["query"] = query,
            ["page"] = "1",
            ["pageSize"] = "100"
        });
    }

    public async Task<List<string>> GetResourceTypesAsync()
    {
        var response = await Http.GetAsync($"{ApiEndpoints.Resources}/types");
        if (!response.IsSuccessStatusCode)
            return [];

        return await ApiResponseReader.ReadCollectionAsync<string>(response);
    }

    public async Task<List<string>> GetLevelsAsync()
    {
        var response = await Http.GetAsync($"{ApiEndpoints.Resources}/levels");
        if (!response.IsSuccessStatusCode)
            return [];

        return await ApiResponseReader.ReadCollectionAsync<string>(response);
    }

    public async Task<ResourceDto?> CreateResourceAsync(CreateResourceRequest request)
    {
        var response = await Http.PostAsJsonAsync(ApiEndpoints.Resources, request);
        if (!response.IsSuccessStatusCode)
            return null;

        return await ApiResponseReader.ReadAsync<ResourceDto>(response);
    }

    public async Task<ResourceDto?> AddResourceAsync(AddResourceRequest request)
    {
        var response = await Http.PostAsJsonAsync(ApiEndpoints.Resources, request);
        if (!response.IsSuccessStatusCode)
            return null;

        return await ApiResponseReader.ReadAsync<ResourceDto>(response);
    }

    public async Task<ResourceDto?> UpdateResourceAsync(Guid id, CreateResourceRequest request)
    {
        var response = await Http.PutAsJsonAsync($"{ApiEndpoints.Resources}/{id}", request);
        if (!response.IsSuccessStatusCode)
            return null;

        return await ApiResponseReader.ReadAsync<ResourceDto>(response);
    }

    public async Task<bool> DeleteResourceAsync(Guid id)
    {
        var response = await Http.DeleteAsync($"{ApiEndpoints.Resources}/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task IncrementViewCountAsync(Guid id)
    {
        await Http.PostAsync($"{ApiEndpoints.Resources}/{id}/views", null);
    }

}

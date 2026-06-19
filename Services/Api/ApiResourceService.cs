using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Models.Responses;
using DotnetNiger.UI.Services.Contracts;
using System.Net.Http.Json;

namespace DotnetNiger.UI.Services.Api;

public class ApiResourceService : IResourceService
{
    private readonly HttpClient _http;
    private const string PublicBase = "api/resources";

    public ApiResourceService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<ResourceDto>> GetAllResourcesAsync()
    {
        return await GetCollectionAsync<ResourceDto>(PublicBase);
    }

    public async Task<ResourceDto?> GetResourceByIdAsync(Guid id)
    {
        var response = await _http.GetAsync($"{PublicBase}/{id}");
        if (!response.IsSuccessStatusCode)
            return null;

        return await ApiResponseReader.ReadAsync<ResourceDto>(response);
    }

    public async Task<ResourceDto?> GetResourceBySlugAsync(string slug)
    {
        var response = await _http.GetAsync($"{PublicBase}/{slug}");
        if (!response.IsSuccessStatusCode)
            return null;

        return await ApiResponseReader.ReadAsync<ResourceDto>(response);
    }

    public async Task<List<ResourceDto>> GetResourcesByTypeAsync(string resourceType)
    {
        var resources = await GetCollectionAsync<ResourceDto>(PublicBase, new Dictionary<string, string?>
        {
            ["resourceType"] = resourceType
        });

        return resources.Where(r => r.ResourceType.Equals(resourceType, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public async Task<List<ResourceDto>> GetResourcesByLevelAsync(string level)
    {
        var resources = await GetCollectionAsync<ResourceDto>(PublicBase, new Dictionary<string, string?>
        {
            ["level"] = level
        });

        return resources.Where(r => r.Level.Equals(level, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public async Task<List<ResourceDto>> SearchResourcesAsync(string query)
    {
        return await GetCollectionAsync<ResourceDto>(PublicBase, new Dictionary<string, string?>
        {
            ["query"] = query,
            ["page"] = "1",
            ["pageSize"] = "100"
        });
    }

    public async Task<List<string>> GetResourceTypesAsync()
    {
        var response = await _http.GetAsync($"{PublicBase}/types");
        if (!response.IsSuccessStatusCode)
            return [];

        return await ApiResponseReader.ReadCollectionAsync<string>(response);
    }

    public async Task<List<string>> GetLevelsAsync()
    {
        var response = await _http.GetAsync($"{PublicBase}/levels");
        if (!response.IsSuccessStatusCode)
            return [];

        return await ApiResponseReader.ReadCollectionAsync<string>(response);
    }

    public async Task<ResourceDto?> CreateResourceAsync(CreateResourceRequest request)
    {
        var response = await _http.PostAsJsonAsync(PublicBase, request);
        if (!response.IsSuccessStatusCode)
            return null;

        return await ApiResponseReader.ReadAsync<ResourceDto>(response);
    }

    public async Task<ResourceDto?> AddResourceAsync(AddResourceRequest request)
    {
        var response = await _http.PostAsJsonAsync(PublicBase, request);
        if (!response.IsSuccessStatusCode)
            return null;

        return await ApiResponseReader.ReadAsync<ResourceDto>(response);
    }

    public async Task<ResourceDto?> UpdateResourceAsync(Guid id, CreateResourceRequest request)
    {
        var response = await _http.PutAsJsonAsync($"{PublicBase}/{id}", request);
        if (!response.IsSuccessStatusCode)
            return null;

        return await ApiResponseReader.ReadAsync<ResourceDto>(response);
    }

    public async Task<bool> DeleteResourceAsync(Guid id)
    {
        var response = await _http.DeleteAsync($"{PublicBase}/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task IncrementViewCountAsync(Guid id)
    {
        await _http.PostAsync($"{PublicBase}/{id}/views", null);
    }

    private async Task<List<T>> GetCollectionAsync<T>(string path, Dictionary<string, string?>? query = null)
    {
        var url = BuildUrl(path, query);
        var response = await _http.GetAsync(url);
        if (!response.IsSuccessStatusCode)
            return new List<T>();

        return await ApiResponseReader.ReadCollectionAsync<T>(response);
    }

    private static string BuildUrl(string path, Dictionary<string, string?>? query = null)
    {
        if (query is null || query.Count == 0)
            return path;

        var queryString = string.Join("&", query
            .Where(kv => !string.IsNullOrWhiteSpace(kv.Value))
            .Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value!)}"));

        return string.IsNullOrWhiteSpace(queryString) ? path : $"{path}?{queryString}";
    }
}

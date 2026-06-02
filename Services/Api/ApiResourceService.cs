using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Models.Responses;
using DotnetNiger.UI.Services.Contracts;
using System.Net.Http.Json;

namespace DotnetNiger.UI.Services.Api;

public class ApiResourceService : IResourceService
{
    private readonly HttpClient _http;
    private const string PublicBase = "api/v1/resources";
    private const string SearchBase = "api/v1/search";

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
        var response = await _http.GetAsync($"{PublicBase}/slug/{slug}");
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
        var searchResults = await GetCollectionAsync<SearchResultDto>(SearchBase, new Dictionary<string, string?>
        {
            ["query"] = query,
            ["type"] = "Resource",
            ["page"] = "1",
            ["pageSize"] = "100"
        });

        var ids = searchResults
            .Where(r => r.Type.Equals("Resource", StringComparison.OrdinalIgnoreCase))
            .Select(r => r.Id)
            .Distinct()
            .ToList();

        if (ids.Count > 0)
        {
            var fetchTasks = ids.Select(GetResourceByIdAsync);
            var resourcesByIds = await Task.WhenAll(fetchTasks);
            return resourcesByIds.Where(r => r is not null).Select(r => r!).ToList();
        }

        var resources = await GetCollectionAsync<ResourceDto>(PublicBase, new Dictionary<string, string?>
        {
            ["query"] = query
        });

        return resources.Where(r =>
                r.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                r.Description.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                r.ResourceType.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToList();
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

    public async Task<ResourceDto> CreateResourceAsync(CreateResourceRequest request)
    {
        var response = await _http.PostAsJsonAsync(PublicBase, request);
        response.EnsureSuccessStatusCode();

        return await ApiResponseReader.ReadAsync<ResourceDto>(response)
               ?? throw new InvalidOperationException("La réponse API est vide pour la création de la ressource.");
    }

    public async Task<ResourceDto> AddResourceAsync(AddResourceRequest request)
    {
        var response = await _http.PostAsJsonAsync(PublicBase, request);
        response.EnsureSuccessStatusCode();

        return await ApiResponseReader.ReadAsync<ResourceDto>(response)
               ?? throw new InvalidOperationException("La réponse API est vide pour l'ajout de la ressource.");
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

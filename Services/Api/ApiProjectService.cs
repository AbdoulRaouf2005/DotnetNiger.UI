using System.Net.Http.Json;
using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Models.Responses;
using DotnetNiger.UI.Services.Contracts;

namespace DotnetNiger.UI.Services.Api;

public class ApiProjectService : IProjectService
{
    private readonly HttpClient _http;
    private const string Base = "api/projects";

    public ApiProjectService(HttpClient http) => _http = http;

    public async Task<PaginatedDto<ProjectResponse>> GetAllAsync(string? status, string? query, int page = 1, int pageSize = 10)
    {
        var q = new Dictionary<string, string?>
        {
            ["page"] = page.ToString(), ["pageSize"] = pageSize.ToString(),
            ["status"] = status, ["query"] = query
        };
        var response = await _http.GetAsync(BuildUrl(Base, q));
        if (!response.IsSuccessStatusCode)
            return new PaginatedDto<ProjectResponse>();
        return await ApiResponseReader.ReadAsync<PaginatedDto<ProjectResponse>>(response)
               ?? new PaginatedDto<ProjectResponse>();
    }

    public async Task<List<ProjectResponse>> GetFeaturedAsync()
    {
        var response = await _http.GetAsync($"{Base}/featured");
        if (!response.IsSuccessStatusCode) return [];
        return await ApiResponseReader.ReadCollectionAsync<ProjectResponse>(response);
    }

    public async Task<ProjectResponse?> GetByIdAsync(Guid id)
    {
        var response = await _http.GetAsync($"{Base}/{id}");
        if (!response.IsSuccessStatusCode) return null;
        return await ApiResponseReader.ReadAsync<ProjectResponse>(response);
    }

    public async Task<ProjectResponse> CreateAsync(CreateProjectRequest request)
    {
        var response = await _http.PostAsJsonAsync(Base, request);
        response.EnsureSuccessStatusCode();
        return await ApiResponseReader.ReadAsync<ProjectResponse>(response)
               ?? throw new InvalidOperationException("Réponse API vide pour la création du projet.");
    }

    public async Task<ProjectResponse?> UpdateAsync(Guid id, UpdateProjectRequest request)
    {
        var response = await _http.PutAsJsonAsync($"{Base}/{id}", request);
        if (!response.IsSuccessStatusCode) return null;
        return await ApiResponseReader.ReadAsync<ProjectResponse>(response);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var response = await _http.DeleteAsync($"{Base}/{id}");
        return response.IsSuccessStatusCode;
    }

    private static string BuildUrl(string path, Dictionary<string, string?> query)
    {
        var qs = string.Join("&", query
            .Where(kv => !string.IsNullOrWhiteSpace(kv.Value))
            .Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value!)}"));
        return string.IsNullOrWhiteSpace(qs) ? path : $"{path}?{qs}";
    }
}

using System.Net.Http.Json;
using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Models.Responses;
using DotnetNiger.UI.Services.Contracts;

namespace DotnetNiger.UI.Services.Api;

public class ApiProjectService : ApiServiceBase, IProjectService
{
    public ApiProjectService(HttpClient http) : base(http)
    {
    }

    public async Task<PaginatedDto<ProjectResponse>> GetAllAsync(string? status, string? query, int page = 1, int pageSize = 10)
    {
        var q = new Dictionary<string, string?>
        {
            ["page"] = page.ToString(), ["pageSize"] = pageSize.ToString(),
            ["status"] = status, ["query"] = query
        };
        var response = await Http.GetAsync(BuildUrl(ApiEndpoints.Projects, q));
        if (!response.IsSuccessStatusCode)
            return new PaginatedDto<ProjectResponse>();
        return await ApiResponseReader.ReadAsync<PaginatedDto<ProjectResponse>>(response)
               ?? new PaginatedDto<ProjectResponse>();
    }

    public async Task<List<ProjectResponse>> GetFeaturedAsync()
    {
        var response = await Http.GetAsync($"{ApiEndpoints.Projects}/featured");
        if (!response.IsSuccessStatusCode) return [];
        return await ApiResponseReader.ReadCollectionAsync<ProjectResponse>(response);
    }

    public async Task<ProjectResponse?> GetByIdAsync(Guid id)
    {
        var response = await Http.GetAsync($"{ApiEndpoints.Projects}/{id}");
        if (!response.IsSuccessStatusCode) return null;
        return await ApiResponseReader.ReadAsync<ProjectResponse>(response);
    }

    public async Task<ProjectResponse?> GetBySlugAsync(string slug)
    {
        var response = await Http.GetAsync($"{ApiEndpoints.Projects}/slug/{slug}");
        if (!response.IsSuccessStatusCode) return null;
        return await ApiResponseReader.ReadAsync<ProjectResponse>(response);
    }

    public async Task<ProjectResponse?> CreateAsync(CreateProjectRequest request)
    {
        var response = await Http.PostAsJsonAsync(ApiEndpoints.Projects, request);
        if (!response.IsSuccessStatusCode)
            return null;
        return await ApiResponseReader.ReadAsync<ProjectResponse>(response);
    }

    public async Task<ProjectResponse?> UpdateAsync(Guid id, UpdateProjectRequest request)
    {
        var response = await Http.PutAsJsonAsync($"{ApiEndpoints.Projects}/{id}", request);
        if (!response.IsSuccessStatusCode) return null;
        return await ApiResponseReader.ReadAsync<ProjectResponse>(response);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var response = await Http.DeleteAsync($"{ApiEndpoints.Projects}/{id}");
        return response.IsSuccessStatusCode;
    }
}

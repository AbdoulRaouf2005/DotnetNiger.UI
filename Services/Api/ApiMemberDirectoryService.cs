using DotnetNiger.UI.Models.Responses;
using DotnetNiger.UI.Services.Contracts;

namespace DotnetNiger.UI.Services.Api;

public class ApiMemberDirectoryService : IMemberDirectoryService
{
    private readonly HttpClient _http;
    private const string Base = "api/members";

    public ApiMemberDirectoryService(HttpClient http) => _http = http;

    public async Task<PaginatedDto<MemberDirectoryResponse>> GetAllAsync(string? query, string? country, int page = 1, int pageSize = 10)
    {
        var q = new Dictionary<string, string?>
        {
            ["page"] = page.ToString(), ["pageSize"] = pageSize.ToString(),
            ["query"] = query, ["country"] = country
        };
        var url = BuildUrl(Base, q);
        var response = await _http.GetAsync(url);
        if (!response.IsSuccessStatusCode)
            return new PaginatedDto<MemberDirectoryResponse>();
        return await ApiResponseReader.ReadAsync<PaginatedDto<MemberDirectoryResponse>>(response)
               ?? new PaginatedDto<MemberDirectoryResponse>();
    }

    public async Task<MemberDirectoryResponse?> GetByIdAsync(Guid id)
    {
        var response = await _http.GetAsync($"{Base}/{id}");
        if (!response.IsSuccessStatusCode) return null;
        return await ApiResponseReader.ReadAsync<MemberDirectoryResponse>(response);
    }

    private static string BuildUrl(string path, Dictionary<string, string?> query)
    {
        var qs = string.Join("&", query
            .Where(kv => !string.IsNullOrWhiteSpace(kv.Value))
            .Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value!)}"));
        return string.IsNullOrWhiteSpace(qs) ? path : $"{path}?{qs}";
    }
}

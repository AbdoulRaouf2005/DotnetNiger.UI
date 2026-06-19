using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Models.Responses;
using DotnetNiger.UI.Services.Contracts;

namespace DotnetNiger.UI.Services.Api;

public class ApiSearchService : ISearchService
{
    private readonly HttpClient _http;
    private const string Base = "api/search";

    public ApiSearchService(HttpClient http) => _http = http;

    public async Task<PaginatedDto<SearchResultDto>> SearchAsync(SearchQueryRequest request)
    {
        var q = new Dictionary<string, string?>
        {
            ["query"] = request.Query,
            ["type"] = request.Type,
            ["page"] = request.Page.ToString(),
            ["pageSize"] = request.PageSize.ToString()
        };
        var url = BuildUrl(Base, q);
        var response = await _http.GetAsync(url);
        if (!response.IsSuccessStatusCode)
            return new PaginatedDto<SearchResultDto>();
        return await ApiResponseReader.ReadAsync<PaginatedDto<SearchResultDto>>(response)
               ?? new PaginatedDto<SearchResultDto>();
    }

    private static string BuildUrl(string path, Dictionary<string, string?> query)
    {
        var qs = string.Join("&", query
            .Where(kv => !string.IsNullOrWhiteSpace(kv.Value))
            .Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value!)}"));
        return string.IsNullOrWhiteSpace(qs) ? path : $"{path}?{qs}";
    }
}

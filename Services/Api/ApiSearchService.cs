using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Models.Responses;
using DotnetNiger.UI.Services.Contracts;

namespace DotnetNiger.UI.Services.Api;

public class ApiSearchService : ApiServiceBase, ISearchService
{
    public ApiSearchService(HttpClient http) : base(http) { }

    public async Task<PaginatedDto<SearchResultDto>> SearchAsync(SearchQueryRequest request)
    {
        var q = new Dictionary<string, string?>
        {
            ["query"] = request.Query,
            ["type"] = request.Type,
            ["page"] = request.Page.ToString(),
            ["pageSize"] = request.PageSize.ToString()
        };
        var url = BuildUrl(ApiEndpoints.Search, q);
        var response = await Http.GetAsync(url);
        if (!response.IsSuccessStatusCode)
            return new PaginatedDto<SearchResultDto>();
        return await ApiResponseReader.ReadAsync<PaginatedDto<SearchResultDto>>(response)
               ?? new PaginatedDto<SearchResultDto>();
    }

}

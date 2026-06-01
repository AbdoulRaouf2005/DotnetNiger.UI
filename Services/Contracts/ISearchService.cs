using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Models.Responses;

namespace DotnetNiger.UI.Services.Contracts;

public interface ISearchService
{
    Task<PaginatedDto<SearchResultDto>> SearchAsync(SearchQueryRequest request);
}

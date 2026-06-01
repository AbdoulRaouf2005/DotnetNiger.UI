using DotnetNiger.UI.Models.Responses;

namespace DotnetNiger.UI.Services.Contracts;

public interface IMemberDirectoryService
{
    Task<PaginatedDto<MemberDirectoryResponse>> GetAllAsync(string? query, string? country, int page = 1, int pageSize = 10);
    Task<MemberDirectoryResponse?> GetByIdAsync(Guid id);
}

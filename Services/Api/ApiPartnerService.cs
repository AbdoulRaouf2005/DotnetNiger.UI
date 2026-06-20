using DotnetNiger.UI.Models.Responses;
using DotnetNiger.UI.Services.Contracts;

namespace DotnetNiger.UI.Services.Api;

public class ApiPartnerService : ApiServiceBase, IPartnerService
{
    public ApiPartnerService(HttpClient http) : base(http) { }

    public async Task<List<PartnerResponse>> GetAllActiveAsync(string? partnerType)
    {
        var url = string.IsNullOrWhiteSpace(partnerType) ? ApiEndpoints.Partners : $"{ApiEndpoints.Partners}?partnerType={Uri.EscapeDataString(partnerType)}";
        var response = await Http.GetAsync(url);
        if (!response.IsSuccessStatusCode) return [];
        return await ApiResponseReader.ReadCollectionAsync<PartnerResponse>(response);
    }

    public async Task<PartnerResponse?> GetByIdAsync(Guid id)
    {
        var response = await Http.GetAsync($"{ApiEndpoints.Partners}/{id}");
        if (!response.IsSuccessStatusCode) return null;
        return await ApiResponseReader.ReadAsync<PartnerResponse>(response);
    }
}

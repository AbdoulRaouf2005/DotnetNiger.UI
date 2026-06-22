using System.Net.Http.Json;
using DotnetNiger.UI.Models.Requests;
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

    public async Task<List<PartnerResponse>> GetAllAsync()
    {
        var response = await Http.GetAsync(ApiEndpoints.Partners);
        if (!response.IsSuccessStatusCode) return [];
        return await ApiResponseReader.ReadCollectionAsync<PartnerResponse>(response);
    }

    public async Task<PartnerResponse?> GetByIdAsync(Guid id)
    {
        var response = await Http.GetAsync($"{ApiEndpoints.Partners}/{id}");
        if (!response.IsSuccessStatusCode) return null;
        return await ApiResponseReader.ReadAsync<PartnerResponse>(response);
    }

    public async Task<PartnerResponse?> CreateAsync(CreatePartnerRequest request)
    {
        var response = await Http.PostAsJsonAsync(ApiEndpoints.Partners, request);
        if (!response.IsSuccessStatusCode) return null;
        return await ApiResponseReader.ReadAsync<PartnerResponse>(response);
    }

    public async Task<PartnerResponse?> UpdateAsync(Guid id, UpdatePartnerRequest request)
    {
        var response = await Http.PutAsJsonAsync($"{ApiEndpoints.Partners}/{id}", request);
        if (!response.IsSuccessStatusCode) return null;
        return await ApiResponseReader.ReadAsync<PartnerResponse>(response);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var response = await Http.DeleteAsync($"{ApiEndpoints.Partners}/{id}");
        return response.IsSuccessStatusCode;
    }
}

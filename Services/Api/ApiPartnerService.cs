using DotnetNiger.UI.Models.Responses;
using DotnetNiger.UI.Services.Contracts;

namespace DotnetNiger.UI.Services.Api;

public class ApiPartnerService : IPartnerService
{
    private readonly HttpClient _http;
    private const string Base = "api/v1/partners";

    public ApiPartnerService(HttpClient http) => _http = http;

    public async Task<List<PartnerResponse>> GetAllActiveAsync(string? partnerType)
    {
        var url = string.IsNullOrWhiteSpace(partnerType) ? Base : $"{Base}?partnerType={Uri.EscapeDataString(partnerType)}";
        var response = await _http.GetAsync(url);
        if (!response.IsSuccessStatusCode) return [];
        return await ApiResponseReader.ReadCollectionAsync<PartnerResponse>(response);
    }

    public async Task<PartnerResponse?> GetByIdAsync(Guid id)
    {
        var response = await _http.GetAsync($"{Base}/{id}");
        if (!response.IsSuccessStatusCode) return null;
        return await ApiResponseReader.ReadAsync<PartnerResponse>(response);
    }
}

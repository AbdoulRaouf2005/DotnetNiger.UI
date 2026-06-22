using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Models.Responses;
using DotnetNiger.UI.Services.Contracts;
using System.Net.Http.Json;

namespace DotnetNiger.UI.Services.Api;

public class ApiProfileService : ApiServiceBase, IProfileService
{
    public ApiProfileService(HttpClient http)
        : base(http)
    {
    }

    public async Task<UserDto> GetProfileAsync()
    {
        var response = await Http.GetAsync(ApiEndpoints.Profile);
        if (!response.IsSuccessStatusCode)
            return new UserDto();

        return await ApiResponseReader.ReadAsync<UserDto>(response) ?? new UserDto();
    }

    public async Task<UserDto> UpdateProfileAsync(UpdateProfileRequest request)
    {
        var response = await Http.PutAsJsonAsync(ApiEndpoints.Profile, request);
        if (!response.IsSuccessStatusCode)
            return await GetProfileAsync();

        if (response.Content.Headers.ContentLength is null or 0)
            return await GetProfileAsync();

        var updated = await ApiResponseReader.ReadAsync<UserDto>(response);
        return updated ?? await GetProfileAsync();
    }

    public async Task<List<SocialLinkDto>> GetSocialLinksAsync()
    {
        var response = await Http.GetAsync(ApiEndpoints.SocialLinks);
        if (!response.IsSuccessStatusCode)
            return new List<SocialLinkDto>();

        return await ApiResponseReader.ReadCollectionAsync<SocialLinkDto>(response);
    }

    public async Task<SocialLinkDto?> AddSocialLinkAsync(AddSocialLinkRequest request)
    {
        var response = await Http.PostAsJsonAsync(ApiEndpoints.SocialLinks, request);
        if (!response.IsSuccessStatusCode)
            return null;

        if (response.Content.Headers.ContentLength is null or 0)
        {
            var links = await GetSocialLinksAsync();
            return links.LastOrDefault(link =>
                link.Platform.Equals(request.Platform, StringComparison.OrdinalIgnoreCase) &&
                link.Url.Equals(request.Url, StringComparison.OrdinalIgnoreCase));
        }

        return await ApiResponseReader.ReadAsync<SocialLinkDto>(response);
    }

    public async Task<bool> DeleteSocialLinkAsync(Guid id)
    {
        var response = await Http.DeleteAsync($"{ApiEndpoints.SocialLinks}/{id}");
        return response.IsSuccessStatusCode;
    }
}

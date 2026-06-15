using System.Net.Http.Json;
using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Models.Responses;
using DotnetNiger.UI.Services.Contracts;

namespace DotnetNiger.UI.Services.Api;

public class ApiProfileService : IProfileService
{
    private readonly HttpClient _http;
    private readonly IUserStateService _userStateService;
    private const string ProfileBase = "api/v1/me";
    private const string SocialLinksBase = "api/v1/social-links";

    public ApiProfileService(HttpClient http, IUserStateService userStateService)
    {
        _http = http;
        _userStateService = userStateService;
    }

    public async Task<UserDto> GetProfileAsync()
    {
        var response = await _http.GetAsync(ProfileBase);
        if (!response.IsSuccessStatusCode)
            return new UserDto();

        return await ApiResponseReader.ReadAsync<UserDto>(response) ?? new UserDto();
    }

    public async Task<UserDto> UpdateProfileAsync(UpdateProfileRequest request)
    {
        var response = await _http.PutAsJsonAsync(ProfileBase, request);
        if (!response.IsSuccessStatusCode)
            return await GetProfileAsync();

        if (response.Content.Headers.ContentLength is null or 0)
            return await GetProfileAsync();

        var updated = await ApiResponseReader.ReadAsync<UserDto>(response);
        var result = updated ?? await GetProfileAsync();

        await _userStateService.UpdateUserAsync(result);
        return result;
    }

    public async Task<List<SocialLinkDto>> GetSocialLinksAsync()
    {
        var response = await _http.GetAsync(SocialLinksBase);
        if (!response.IsSuccessStatusCode)
            return new List<SocialLinkDto>();

        return await ApiResponseReader.ReadCollectionAsync<SocialLinkDto>(response);
    }

    public async Task<SocialLinkDto?> AddSocialLinkAsync(AddSocialLinkRequest request)
    {
        var response = await _http.PostAsJsonAsync(SocialLinksBase, request);
        if (!response.IsSuccessStatusCode)
            return null;

        SocialLinkDto? result;
        if (response.Content.Headers.ContentLength is null or 0)
        {
            var links = await GetSocialLinksAsync();
            result = links.LastOrDefault(link =>
                link.Platform.Equals(request.Platform, StringComparison.OrdinalIgnoreCase) &&
                link.Url.Equals(request.Url, StringComparison.OrdinalIgnoreCase));
        }
        else
        {
            result = await ApiResponseReader.ReadAsync<SocialLinkDto>(response);
        }

        if (result is not null)
        {
            var profile = await GetProfileAsync();
            if (profile.Id != Guid.Empty)
                await _userStateService.UpdateUserAsync(profile);
        }

        return result;
    }

    public async Task<bool> DeleteSocialLinkAsync(Guid id)
    {
        var response = await _http.DeleteAsync($"{SocialLinksBase}/{id}");
        if (response.IsSuccessStatusCode)
        {
            var profile = await GetProfileAsync();
            if (profile.Id != Guid.Empty)
                await _userStateService.UpdateUserAsync(profile);
        }
        return response.IsSuccessStatusCode;
    }
}

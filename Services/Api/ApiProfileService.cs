using System.Net.Http.Headers;
using System.Net.Http.Json;
using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Models.Responses;
using DotnetNiger.UI.Services.Auth;
using DotnetNiger.UI.Services.Contracts;

namespace DotnetNiger.UI.Services.Api;

public class ApiProfileService : ApiServiceBase, IProfileService
{
    private readonly CustomAuthStateProvider _authProvider;

    public ApiProfileService(HttpClient http, CustomAuthStateProvider authProvider)
        : base(http)
    {
        _authProvider = authProvider;
    }

    public async Task<UserDto> GetProfileAsync()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, ApiEndpoints.Profile);
        await AttachAuthHeaderAsync(request);

        var response = await Http.SendAsync(request);
        if (!response.IsSuccessStatusCode)
            return new UserDto();

        return await ApiResponseReader.ReadAsync<UserDto>(response) ?? new UserDto();
    }

    public async Task<UserDto> UpdateProfileAsync(UpdateProfileRequest request)
    {
        using var httpRequest = new HttpRequestMessage(HttpMethod.Put, ApiEndpoints.Profile)
        {
            Content = JsonContent.Create(request)
        };
        await AttachAuthHeaderAsync(httpRequest);

        var response = await Http.SendAsync(httpRequest);
        if (!response.IsSuccessStatusCode)
            return await GetProfileAsync();

        if (response.Content.Headers.ContentLength is null or 0)
            return await GetProfileAsync();

        var updated = await ApiResponseReader.ReadAsync<UserDto>(response);
        return updated ?? await GetProfileAsync();
    }

    public async Task<List<SocialLinkDto>> GetSocialLinksAsync()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, ApiEndpoints.SocialLinks);
        await AttachAuthHeaderAsync(request);

        var response = await Http.SendAsync(request);
        if (!response.IsSuccessStatusCode)
            return new List<SocialLinkDto>();

        return await ApiResponseReader.ReadCollectionAsync<SocialLinkDto>(response);
    }

    public async Task<SocialLinkDto?> AddSocialLinkAsync(AddSocialLinkRequest request)
    {
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, ApiEndpoints.SocialLinks)
        {
            Content = JsonContent.Create(request)
        };
        await AttachAuthHeaderAsync(httpRequest);

        var response = await Http.SendAsync(httpRequest);
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
        using var request = new HttpRequestMessage(HttpMethod.Delete, $"{ApiEndpoints.SocialLinks}/{id}");
        await AttachAuthHeaderAsync(request);

        var response = await Http.SendAsync(request);
        return response.IsSuccessStatusCode;
    }

    private async Task AttachAuthHeaderAsync(HttpRequestMessage request)
    {
        var token = await _authProvider.GetAccessTokenAsync();
        if (string.IsNullOrWhiteSpace(token))
            return;

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}

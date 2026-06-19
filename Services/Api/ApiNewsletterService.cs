using System.Net.Http.Json;
using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Services.Contracts;

namespace DotnetNiger.UI.Services.Api;

public class ApiNewsletterService : INewsletterService
{
    private readonly HttpClient _http;
    private const string Base = "api/newsletters";

    public ApiNewsletterService(HttpClient http) => _http = http;

    public async Task<bool> SubscribeAsync(SubscribeRequest request)
    {
        var response = await _http.PostAsJsonAsync($"{Base}/subscribe", request);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UnsubscribeAsync(UnsubscribeRequest request)
    {
        var response = await _http.PostAsJsonAsync($"{Base}/unsubscribe", request);
        return response.IsSuccessStatusCode;
    }
}

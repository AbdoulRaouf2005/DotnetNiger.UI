using System.Net.Http.Json;
using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Services.Contracts;

namespace DotnetNiger.UI.Services.Api;

public class ApiContactService : IContactService
{
    private readonly HttpClient _http;

    public ApiContactService(HttpClient http) => _http = http;

    public async Task<bool> SendAsync(ContactRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/contact", request);
        return response.IsSuccessStatusCode;
    }
}

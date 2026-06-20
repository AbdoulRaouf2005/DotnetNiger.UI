using System.Net.Http.Json;
using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Services.Contracts;

namespace DotnetNiger.UI.Services.Api;

public class ApiContactService : ApiServiceBase, IContactService
{
    public ApiContactService(HttpClient http) : base(http)
    {
    }

    public async Task<bool> SendAsync(ContactRequest request)
    {
        var response = await Http.PostAsJsonAsync(ApiEndpoints.Contact, request);
        return response.IsSuccessStatusCode;
    }
}

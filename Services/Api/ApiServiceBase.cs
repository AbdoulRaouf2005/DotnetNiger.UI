using System.Net.Http.Json;

namespace DotnetNiger.UI.Services.Api;

public abstract class ApiServiceBase
{
    protected readonly HttpClient Http;

    protected ApiServiceBase(HttpClient http)
    {
        Http = http;
    }

    protected async Task<List<T>> GetCollectionAsync<T>(string path, Dictionary<string, string?>? query = null)
    {
        var url = BuildUrl(path, query);
        var response = await Http.GetAsync(url);
        if (!response.IsSuccessStatusCode)
            return new List<T>();

        return await ApiResponseReader.ReadCollectionAsync<T>(response);
    }

    protected static string BuildUrl(string path, Dictionary<string, string?>? query = null)
    {
        if (query is null || query.Count == 0)
            return path;

        var queryString = string.Join("&", query
            .Where(kv => !string.IsNullOrWhiteSpace(kv.Value))
            .Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value!)}"));

        return string.IsNullOrWhiteSpace(queryString) ? path : $"{path}?{queryString}";
    }
}

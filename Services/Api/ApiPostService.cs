using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Models.Responses;
using DotnetNiger.UI.Services.Contracts;
using System.Net.Http.Json;

namespace DotnetNiger.UI.Services.Api;

public class ApiPostService : IPostService
{
    private readonly HttpClient _http;
    private const string PublicBase = "api/v1/posts";

    public ApiPostService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<PostDto>> GetAllPostsAsync()
    {
        return await GetCollectionAsync<PostDto>(PublicBase);
    }

    public async Task<List<PostDto>> GetPublishedPostsAsync()
    {
        var posts = await GetCollectionAsync<PostDto>(PublicBase, new Dictionary<string, string?>
        {
            ["published"] = "true"
        });

        return posts.Where(p => p.PublishedAt != DateTime.MinValue).ToList();
    }

    public async Task<List<PostDto>> GetPostsByCategoryAsync(string categorySlug)
    {
        var posts = await GetCollectionAsync<PostDto>(PublicBase, new Dictionary<string, string?>
        {
            ["category"] = categorySlug
        });

        return posts
            .Where(p => p.Categories.Any(c => c.Slug.Equals(categorySlug, StringComparison.OrdinalIgnoreCase)))
            .ToList();
    }

    public async Task<List<PostDto>> GetPostsByTagAsync(string tagSlug)
    {
        var posts = await GetCollectionAsync<PostDto>(PublicBase, new Dictionary<string, string?>
        {
            ["tag"] = tagSlug
        });

        return posts
            .Where(p => p.Tags.Any(t => t.Slug.Equals(tagSlug, StringComparison.OrdinalIgnoreCase)))
            .ToList();
    }

    public async Task<PostDto?> GetPostByIdAsync(Guid id)
    {
        var response = await _http.GetAsync($"{PublicBase}/{id}");
        if (!response.IsSuccessStatusCode)
            return null;

        return await ApiResponseReader.ReadAsync<PostDto>(response);
    }

    public async Task<PostDto?> GetPostBySlugAsync(string slug)
    {
        var response = await _http.GetAsync($"{PublicBase}/{slug}");
        if (!response.IsSuccessStatusCode)
            return null;

        return await ApiResponseReader.ReadAsync<PostDto>(response);
    }

    public async Task<PostDto> CreatePostAsync(CreatePostRequest request , Guid currentId)
    {
        var response = await _http.PostAsJsonAsync(PublicBase, request);
        response.EnsureSuccessStatusCode();

        return await ApiResponseReader.ReadAsync<PostDto>(response)
               ?? throw new InvalidOperationException("La réponse API est vide pour la création du post.");
    }

    public async Task<PostDto?> UpdatePostAsync(Guid id, UpdatePostRequest request)
    {
        var response = await _http.PutAsJsonAsync($"{PublicBase}/{id}", request);
        if (!response.IsSuccessStatusCode)
            return null;

        return await ApiResponseReader.ReadAsync<PostDto>(response);
    }

    public async Task<bool> DeletePostAsync(Guid id)
    {
        var response = await _http.DeleteAsync($"{PublicBase}/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<List<PostDto>> SearchPostsAsync(string query)
    {
        return await GetCollectionAsync<PostDto>(PublicBase, new Dictionary<string, string?>
        {
            ["query"] = query,
            ["page"] = "1",
            ["pageSize"] = "100"
        });
    }

    public async Task IncrementViewCountAsync(Guid id)
    {
        await _http.PostAsync($"{PublicBase}/{id}/views", null);
    }

    private async Task<List<T>> GetCollectionAsync<T>(string path, Dictionary<string, string?>? query = null)
    {
        var url = BuildUrl(path, query);
        var response = await _http.GetAsync(url);
        if (!response.IsSuccessStatusCode)
            return new List<T>();

        return await ApiResponseReader.ReadCollectionAsync<T>(response);
    }

    private static string BuildUrl(string path, Dictionary<string, string?>? query = null)
    {
        if (query is null || query.Count == 0)
            return path;

        var queryString = string.Join("&", query
            .Where(kv => !string.IsNullOrWhiteSpace(kv.Value))
            .Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value!)}"));

        return string.IsNullOrWhiteSpace(queryString) ? path : $"{path}?{queryString}";
    }
}

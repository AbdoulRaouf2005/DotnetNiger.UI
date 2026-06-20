using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Models.Responses;
using DotnetNiger.UI.Services.Contracts;
using System.Net.Http.Json;

namespace DotnetNiger.UI.Services.Api;

public class ApiPostService : ApiServiceBase, IPostService
{
    public ApiPostService(HttpClient http) : base(http)
    {
    }

    public async Task<List<PostDto>> GetAllPostsAsync()
    {
        return await GetCollectionAsync<PostDto>(ApiEndpoints.Posts);
    }

    public async Task<List<PostDto>> GetPublishedPostsAsync()
    {
        var posts = await GetCollectionAsync<PostDto>(ApiEndpoints.Posts, new Dictionary<string, string?>
        {
            ["published"] = "true"
        });

        return posts.Where(p => p.PublishedAt != DateTime.MinValue).ToList();
    }

    public async Task<List<PostDto>> GetPostsByCategoryAsync(string categorySlug)
    {
        var posts = await GetCollectionAsync<PostDto>(ApiEndpoints.Posts, new Dictionary<string, string?>
        {
            ["category"] = categorySlug
        });

        return posts
            .Where(p => p.Categories.Any(c => c.Slug.Equals(categorySlug, StringComparison.OrdinalIgnoreCase)))
            .ToList();
    }

    public async Task<List<PostDto>> GetPostsByTagAsync(string tagSlug)
    {
        var posts = await GetCollectionAsync<PostDto>(ApiEndpoints.Posts, new Dictionary<string, string?>
        {
            ["tag"] = tagSlug
        });

        return posts
            .Where(p => p.Tags.Any(t => t.Slug.Equals(tagSlug, StringComparison.OrdinalIgnoreCase)))
            .ToList();
    }

    public async Task<PostDto?> GetPostByIdAsync(Guid id)
    {
        var response = await Http.GetAsync($"{ApiEndpoints.Posts}/{id}");
        if (!response.IsSuccessStatusCode)
            return null;

        return await ApiResponseReader.ReadAsync<PostDto>(response);
    }

    public async Task<PostDto?> GetPostBySlugAsync(string slug)
    {
        var response = await Http.GetAsync($"{ApiEndpoints.Posts}/{slug}");
        if (!response.IsSuccessStatusCode)
            return null;

        return await ApiResponseReader.ReadAsync<PostDto>(response);
    }

    public async Task<PostDto?> CreatePostAsync(CreatePostRequest request , Guid currentId)
    {
        var response = await Http.PostAsJsonAsync(ApiEndpoints.Posts, request);
        if (!response.IsSuccessStatusCode)
            return null;

        return await ApiResponseReader.ReadAsync<PostDto>(response);
    }

    public async Task<PostDto?> UpdatePostAsync(Guid id, UpdatePostRequest request)
    {
        var response = await Http.PutAsJsonAsync($"{ApiEndpoints.Posts}/{id}", request);
        if (!response.IsSuccessStatusCode)
            return null;

        return await ApiResponseReader.ReadAsync<PostDto>(response);
    }

    public async Task<bool> DeletePostAsync(Guid id)
    {
        var response = await Http.DeleteAsync($"{ApiEndpoints.Posts}/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<List<PostDto>> SearchPostsAsync(string query)
    {
        return await GetCollectionAsync<PostDto>(ApiEndpoints.Posts, new Dictionary<string, string?>
        {
            ["query"] = query,
            ["page"] = "1",
            ["pageSize"] = "100"
        });
    }

    public async Task IncrementViewCountAsync(Guid id)
    {
        await Http.PostAsync($"{ApiEndpoints.Posts}/{id}/views", null);
    }
}

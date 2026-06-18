using System.Net.Http.Json;
using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Models.Responses;
using DotnetNiger.UI.Services.Auth;
using DotnetNiger.UI.Services.Contracts;

namespace DotnetNiger.UI.Services.Api;

public class ApiCommentService : ICommentService
{
    private readonly HttpClient _http;
    private readonly CustomAuthStateProvider _authProvider;

    public ApiCommentService(HttpClient http, CustomAuthStateProvider authProvider)
    {
        _http = http;
        _authProvider = authProvider;
    }

    public Guid CurrentUserId { get; private set; }

    public async Task<List<CommentResponse>> GetCommentsByPostIdAsync(Guid postId)
    {
        var response = await _http.GetAsync($"api/comments/post/{postId}");
        if (!response.IsSuccessStatusCode)
            return [];

        return await ApiResponseReader.ReadCollectionAsync<CommentResponse>(response);
    }

    public async Task<List<CommentResponse>> GetCommentsByEventIdAsync(Guid eventId)
    {
        var response = await _http.GetAsync($"api/comments/event/{eventId}");
        if (!response.IsSuccessStatusCode)
            return [];

        return await ApiResponseReader.ReadCollectionAsync<CommentResponse>(response);
    }

    public async Task<CommentResponse?> GetCommentByIdAsync(Guid id)
    {
        var response = await _http.GetAsync($"api/comments/{id}");
        if (!response.IsSuccessStatusCode)
            return null;

        return await ApiResponseReader.ReadAsync<CommentResponse>(response);
    }

    public async Task<CommentResponse> CreateCommentAsync(CreateCommentRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/comments", request);
        response.EnsureSuccessStatusCode();

        return await ApiResponseReader.ReadAsync<CommentResponse>(response)
               ?? throw new InvalidOperationException("Empty API response for comment creation.");
    }

    public async Task<CommentResponse?> UpdateCommentAsync(UpdateCommentRequest request)
    {
        var response = await _http.PutAsJsonAsync($"api/comments/{request.Id}", new { content = request.Content });
        if (!response.IsSuccessStatusCode)
            return null;

        return await ApiResponseReader.ReadAsync<CommentResponse>(response);
    }

    public async Task<bool> DeleteCommentAsync(DeleteCommentRequest request)
    {
        var url = request.DeleteAllReplies
            ? $"api/comments/{request.Id}?deleteAllReplies=true"
            : $"api/comments/{request.Id}";

        var response = await _http.DeleteAsync(url);
        return response.IsSuccessStatusCode;
    }
}

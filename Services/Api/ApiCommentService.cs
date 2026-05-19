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
        var response = await _http.GetAsync($"api/v1/comments/post/{postId}");
        if (!response.IsSuccessStatusCode)
            return [];

        return await ApiResponseReader.ReadCollectionAsync<CommentResponse>(response);
    }

    public async Task<List<CommentResponse>> GetCommentsByEventIdAsync(Guid eventId)
    {
        var response = await _http.GetAsync($"api/v1/comments/event/{eventId}");
        if (!response.IsSuccessStatusCode)
            return [];

        return await ApiResponseReader.ReadCollectionAsync<CommentResponse>(response);
    }

    public async Task<CommentResponse?> GetCommentByIdAsync(Guid id)
    {
        var response = await _http.GetAsync($"api/v1/comments/{id}");
        if (!response.IsSuccessStatusCode)
            return null;

        return await ApiResponseReader.ReadAsync<CommentResponse>(response);
    }

    public async Task<CommentResponse> CreateCommentAsync(CreateCommentRequest request)
    {
        var payload = new
        {
            content = request.Content,
            postId = ParseGuidOrNull(request.PostId),
            eventId = ParseGuidOrNull(request.EventId),
            parentCommentId = ParseGuidOrNull(request.ParentCommentId)
        };

        var response = await _http.PostAsJsonAsync("api/v1/comments", payload);
        response.EnsureSuccessStatusCode();

        return await ApiResponseReader.ReadAsync<CommentResponse>(response)
               ?? throw new InvalidOperationException("Empty API response for comment creation.");
    }

    public async Task<CommentResponse?> UpdateCommentAsync(UpdateCommentRequest request)
    {
        if (!Guid.TryParse(request.Id, out var commentId))
            return null;

        var payload = new { content = request.Content };
        var response = await _http.PutAsJsonAsync($"api/v1/comments/{commentId}", payload);
        if (!response.IsSuccessStatusCode)
            return null;

        return await ApiResponseReader.ReadAsync<CommentResponse>(response);
    }

    public async Task<bool> DeleteCommentAsync(DeleteCommentRequest request)
    {
        if (!Guid.TryParse(request.Id, out var commentId))
            return false;

        var url = request.DeleteAllReplies
            ? $"api/v1/comments/{commentId}?deleteAllReplies=true"
            : $"api/v1/comments/{commentId}";

        var response = await _http.DeleteAsync(url);
        return response.IsSuccessStatusCode;
    }

    private static Guid? ParseGuidOrNull(string value) =>
        Guid.TryParse(value, out var guid) && guid != Guid.Empty ? guid : null;
}

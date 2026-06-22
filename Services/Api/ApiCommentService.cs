using System.Net.Http.Json;
using System.Security.Claims;
using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Models.Responses;
using DotnetNiger.UI.Services.Auth;
using DotnetNiger.UI.Services.Contracts;

namespace DotnetNiger.UI.Services.Api;

public class ApiCommentService : ApiServiceBase, ICommentService
{
    private readonly CustomAuthStateProvider _authProvider;
    private Guid? _currentUserId;

    public ApiCommentService(HttpClient http, CustomAuthStateProvider authProvider) : base(http)
    {
        _authProvider = authProvider;
    }

    public Guid CurrentUserId
    {
        get
        {
            if (_currentUserId.HasValue)
                return _currentUserId.Value;

            var tokenTask = _authProvider.GetAccessTokenAsync();
            tokenTask.Wait();
            var token = tokenTask.Result;

            if (string.IsNullOrWhiteSpace(token))
                return Guid.Empty;

            var claims = JwtParser.ParseClaimsFromJwt(token);
            var userIdClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "sub");
            _currentUserId = userIdClaim is not null && Guid.TryParse(userIdClaim.Value, out var uid) ? uid : Guid.Empty;
            return _currentUserId.Value;
        }
    }

    public async Task<List<CommentResponse>> GetCommentsByPostIdAsync(Guid postId)
    {
        var response = await Http.GetAsync($"{ApiEndpoints.Comments}/post/{postId}");
        if (!response.IsSuccessStatusCode)
            return [];

        return await ApiResponseReader.ReadCollectionAsync<CommentResponse>(response);
    }

    public async Task<List<CommentResponse>> GetCommentsByEventIdAsync(Guid eventId)
    {
        var response = await Http.GetAsync($"{ApiEndpoints.Comments}/event/{eventId}");
        if (!response.IsSuccessStatusCode)
            return [];

        return await ApiResponseReader.ReadCollectionAsync<CommentResponse>(response);
    }

    public async Task<CommentResponse?> GetCommentByIdAsync(Guid id)
    {
        var response = await Http.GetAsync($"{ApiEndpoints.Comments}/{id}");
        if (!response.IsSuccessStatusCode)
            return null;

        return await ApiResponseReader.ReadAsync<CommentResponse>(response);
    }

    public async Task<CommentResponse?> CreateCommentAsync(CreateCommentRequest request)
    {
        var response = await Http.PostAsJsonAsync(ApiEndpoints.Comments, request);
        if (!response.IsSuccessStatusCode)
            return null;

        return await ApiResponseReader.ReadAsync<CommentResponse>(response);
    }

    public async Task<CommentResponse?> UpdateCommentAsync(UpdateCommentRequest request)
    {
        var response = await Http.PutAsJsonAsync($"{ApiEndpoints.Comments}/{request.Id}", new { content = request.Content });
        if (!response.IsSuccessStatusCode)
            return null;

        return await ApiResponseReader.ReadAsync<CommentResponse>(response);
    }

    public async Task<bool> DeleteCommentAsync(DeleteCommentRequest request)
    {
        var url = request.DeleteAllReplies
            ? $"{ApiEndpoints.Comments}/{request.Id}?deleteAllReplies=true"
            : $"{ApiEndpoints.Comments}/{request.Id}";

        var response = await Http.DeleteAsync(url);
        return response.IsSuccessStatusCode;
    }
}

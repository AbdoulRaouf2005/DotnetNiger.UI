using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Models.Responses;

namespace DotnetNiger.UI.Services.Contracts;

public interface IPostService
{
    Task<List<PostDto>> GetPublishedPostsAsync();
    Task<List<PostDto>> GetPostsByCategoryAsync(string categorySlug);
    Task<List<PostDto>> GetPostsByTagAsync(string tagSlug);
    Task<List<PostDto>> GetAllPostsAsync();
    Task<PostDto?> GetPostByIdAsync(Guid postId);
    Task<PostDto?> GetPostBySlugAsync(string slug);
    Task<List<PostDto>> SearchPostsAsync(string query);
    Task<PostDto?> CreatePostAsync(CreatePostRequest request,Guid UserId);
    Task<PostDto?> UpdatePostAsync(Guid postId, UpdatePostRequest request);
    Task<bool> DeletePostAsync(Guid postId);
    Task<bool> PublishPostAsync(Guid postId) => Task.FromResult(false);
    Task<bool> UnPublishPostAsync(Guid postId) => Task.FromResult(false);
    Task IncrementViewCountAsync(Guid id);
}

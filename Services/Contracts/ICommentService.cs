using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Models.Responses;

namespace DotnetNiger.UI.Services.Contracts;

public interface ICommentService
{
    Guid CurrentUserId { get; }
    Task<List<CommentResponse>> GetCommentsByPostIdAsync(Guid postId);
    Task<List<CommentResponse>> GetCommentsByEventIdAsync(Guid eventId);
    Task<CommentResponse?> GetCommentByIdAsync(Guid id);
    Task<CommentResponse> CreateCommentAsync(CreateCommentRequest request);
    Task<CommentResponse?> UpdateCommentAsync(UpdateCommentRequest request);
    Task<bool> DeleteCommentAsync(DeleteCommentRequest request);
}

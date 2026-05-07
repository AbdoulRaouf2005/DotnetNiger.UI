using System.ComponentModel.DataAnnotations;

namespace DotnetNiger.UI.Models.Requests;

public class CreateCommentRequest
{
    [Required]
    public string Content { get; set; } = string.Empty;
    public string PostId { get; set; } = string.Empty;
    public string EventId { get; set; } = string.Empty;
    public string ParentCommentId { get; set; } = string.Empty;

}

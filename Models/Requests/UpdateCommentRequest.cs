using System.ComponentModel.DataAnnotations;

namespace DotnetNiger.UI.Models.Requests;

public class UpdateCommentRequest
{
    public string Content { get; set; } = string.Empty;
    [Required]
    public Guid Id { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace DotnetNiger.UI.Models.Requests;

public class UpdateCommentRequest
{
    [Required]
    public string? Content { get; set; }
    [Required]
    public string Id { get; set;} = string.Empty;
}

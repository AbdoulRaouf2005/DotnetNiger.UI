using System.ComponentModel.DataAnnotations;

namespace DotnetNiger.UI.Models.Requests;

public class CreateProjectRequest
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    public string GithubUrl { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string Technologies { get; set; } = string.Empty;
    public string Status { get; set; } = "active";
    public bool IsFeatured { get; set; }
    public bool IsPublished { get; set; }
}

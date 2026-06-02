namespace DotnetNiger.UI.Models.Responses;

public class SearchResultDto
{
    public string Type { get; set; } = string.Empty; // Post, Event, Resource, Project
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDateTime { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Excerpt { get; set; } = string.Empty;
    public string CoverImageUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

namespace DotnetNiger.UI.Models.Responses;

public class ResourceDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string ResourceType { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public Guid CreatedBy { get; set; }
    public int ViewCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<Guid> CategoryIds { get; set; } = [];
    public List<TagDto> Tags { get; set; } = [];
}

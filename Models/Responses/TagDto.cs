namespace DotnetNiger.UI.Models.Responses;

public class TagDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int UsageCount { get; set; }
}

namespace DotnetNiger.UI.Models;

public sealed class DashboardMetric
{
    public required string IconClass { get; set; }
    public int Value { get; set; }
    public required string Label { get; set; }
    public required string Url { get; set; }
}

public sealed class DashboardItem
{
    public DashboardItemKind Kind { get; set; }
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PrimaryRole { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public string CoverImageUrl { get; set; } = string.Empty;
    public string ViewRoute { get; set; } = string.Empty;
    public string EditRoute { get; set; } = string.Empty;
    public string ManageRoute { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
    public bool IsArchived { get; set; }
}

public enum DashboardItemKind
{
    User,
    Post,
    Event,
    Resource,
    Project
}

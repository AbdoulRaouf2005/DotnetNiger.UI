namespace DotnetNiger.UI.Models.Responses;

public class EventDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string CoverImageUrl { get; set; } = string.Empty;
    public Guid CreatedBy { get; set; }
    public string OrganizerName { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public int RegisteredCount { get; set; }
    public bool IsPublished { get; set; }
    public string MeetupLink { get; set; } = string.Empty;
    public List<EventMediaDto> Medias { get; set; } = new();
    public List<string> GalleryImageUrls { get; set; } = new();

    public Guid SubmittedBy { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<TagDto> Tags { get; set; } = new();
    public List<SpeakerDto> Speakers { get; set; } = new();
}

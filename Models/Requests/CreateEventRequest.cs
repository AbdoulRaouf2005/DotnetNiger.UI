using System.ComponentModel.DataAnnotations;
using DotnetNiger.UI.Models.Responses;

namespace DotnetNiger.UI.Models.Requests;

public class CreateEventRequest
{
    [Required]
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    [Required]
    public string EventType { get; set; } = string.Empty; // Online, Physical, Hybrid

    [Required]
    public string Category { get; set; } = string.Empty;

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    public string CoverImageUrl { get; set; } = string.Empty;
    public int Capacity { get; set; } = 100;
    public string MeetupLink { get; set; } = string.Empty;

    public bool IsPublished { get; set; }
    public bool IsArchived { get; set; }
    public List<string> TagNames { get; set; } = new();
    public List<string> GalleryImageUrls { get; set; } = new();
    public List<SpeakerDto> Speakers { get; set; } = new();
}

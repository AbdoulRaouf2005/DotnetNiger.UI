// DTO response Identity: UserDto
namespace DotnetNiger.UI.Models.Responses;

public class UserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsTeamMember { get; set; }
    public string Position { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public List<string> Skills { get; set; } = new();
    public List<string> Roles { get; set; } = new();
    public List<SocialLinkDto> SocialLinks { get; set; } = new();
}

namespace DotnetNiger.UI.Models.Responses;

public class MemberDirectoryResponse
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<SocialLinkDto> SocialLinks { get; set; } = new();
}

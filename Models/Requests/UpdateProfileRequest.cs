// DTO request Identity: UpdateProfileRequest
namespace DotnetNiger.UI.Models.Requests;

// Mise a jour partielle du profil. Seuls les champs non-null sont appliques.
public class UpdateProfileRequest
{
    
    public string? FullName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
}

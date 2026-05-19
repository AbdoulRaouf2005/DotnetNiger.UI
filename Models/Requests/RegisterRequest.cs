// DTO request Identity: RegisterRequest
using System.ComponentModel.DataAnnotations;

namespace DotnetNiger.UI.Models.Requests;

public class RegisterRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    public string Provider { get; set; } = "Email"; // "Email", "Google", "GitHub", "Microsoft"
}

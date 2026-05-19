// DTO request Identity: LoginRequest
using System.ComponentModel.DataAnnotations;

namespace DotnetNiger.UI.Models.Requests;

public class LoginRequest
{
    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
    public string Provider { get; set; } = "Email"; // "Email", "Google", "GitHub", "Microsoft"
}

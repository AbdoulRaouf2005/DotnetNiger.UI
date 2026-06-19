using System.ComponentModel.DataAnnotations;

namespace DotnetNiger.UI.Models.Requests;

public class ContactRequest
{
    [Required(ErrorMessage = "Le nom est requis")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "L'email est requis")]
    [EmailAddress(ErrorMessage = "Email invalide")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le sujet est requis")]
    public string Subject { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le message est requis")]
    [MinLength(10, ErrorMessage = "Le message doit contenir au moins 10 caractères")]
    public string Message { get; set; } = string.Empty;
}

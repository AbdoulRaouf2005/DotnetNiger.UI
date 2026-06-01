// DTO request Identity: VerifyEmailRequest
using System.ComponentModel.DataAnnotations;

namespace DotnetNiger.UI.Models.Requests;

// Requete de verification d'email.
public class VerifyEmailRequest
{
	[Required]
	public string Email { get; set; } = string.Empty;

	[Required]
	public string Code { get; set; } = string.Empty;
}

using System.ComponentModel.DataAnnotations;

namespace DotnetNiger.UI.Models.Requests;

public class CertificateSubmissionDto
{
    public Guid UserId { get; set; }

    [Required]
    public string CertificateUrl { get; set; } = string.Empty;

    [Required]
    public string CertificateType { get; set; } = string.Empty;

    public string SubmissionStatus { get; set; } = "Pending";
}

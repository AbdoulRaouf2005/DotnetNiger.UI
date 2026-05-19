namespace DotnetNiger.UI.Models.Responses;

public class CertificateStatusDto
{
    public string Status { get; set; } = string.Empty; // "Pending", "Validated", "ActionRequired"
    public DateTime SubmissionDate { get; set; }
    public string EstimatedWaitTime { get; set; } = string.Empty; // e.g., "24-48 heures"
    public string SupportEmail { get; set; } = string.Empty;
}
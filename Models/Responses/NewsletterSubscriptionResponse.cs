namespace DotnetNiger.UI.Models.Responses;

public class NewsletterSubscriptionResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime SubscribedAt { get; set; }
}

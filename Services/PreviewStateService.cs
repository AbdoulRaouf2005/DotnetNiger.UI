using DotnetNiger.UI.Models.Responses;

namespace DotnetNiger.UI.Services;

public class PreviewStateService
{
    public PostDto? BlogPreview { get; set; }
    public EventDto? EventPreview { get; set; }
    public string? ReturnUrl { get; set; }

    public void SetBlogPreview(PostDto post, string returnUrl)
    {
        BlogPreview = post;
        EventPreview = null;
        ReturnUrl = returnUrl;
    }

    public void SetEventPreview(EventDto ev, string returnUrl)
    {
        EventPreview = ev;
        BlogPreview = null;
        ReturnUrl = returnUrl;
    }

    public void Clear()
    {
        BlogPreview = null;
        EventPreview = null;
        ReturnUrl = null;
    }
}

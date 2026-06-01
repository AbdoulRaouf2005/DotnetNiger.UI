using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Services.Contracts;

namespace DotnetNiger.UI.Services.Mock;

public class MockNewsletterService : INewsletterService
{
    private readonly List<string> _subscribers = new();

    public Task<bool> SubscribeAsync(SubscribeRequest request)
    {
        if (_subscribers.Contains(request.Email, StringComparer.OrdinalIgnoreCase))
            return Task.FromResult(false);
        _subscribers.Add(request.Email);
        return Task.FromResult(true);
    }

    public Task<bool> UnsubscribeAsync(UnsubscribeRequest request)
    {
        var removed = _subscribers.RemoveAll(e =>
            e.Equals(request.Email, StringComparison.OrdinalIgnoreCase)) > 0;
        return Task.FromResult(removed);
    }
}

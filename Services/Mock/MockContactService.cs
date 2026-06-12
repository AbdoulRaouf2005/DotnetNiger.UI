using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Services.Contracts;

namespace DotnetNiger.UI.Services.Mock;

public class MockContactService : IContactService
{
    private readonly List<ContactRequest> _messages = new();

    public Task<bool> SendAsync(ContactRequest request)
    {
        _messages.Add(request);
        return Task.FromResult(true);
    }
}

using DotnetNiger.UI.Models.Requests;

namespace DotnetNiger.UI.Services.Contracts;

public interface IContactService
{
    Task<bool> SendAsync(ContactRequest request);
}

using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Models.Responses;

namespace DotnetNiger.UI.Services.Contracts;

public interface INewsletterService
{
    Task<bool> SubscribeAsync(SubscribeRequest request);
    Task<bool> UnsubscribeAsync(UnsubscribeRequest request);
}

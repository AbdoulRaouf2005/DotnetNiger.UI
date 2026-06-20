using System.Net.Http.Json;
using DotnetNiger.UI.Models.Responses;
using DotnetNiger.UI.Services.Contracts;

namespace DotnetNiger.UI.Services.Api;

public class ApiNotificationService : ApiServiceBase, INotificationService
{
    public ApiNotificationService(HttpClient http) : base(http) { }

    public event Action<Guid>? NotificationsChanged;

    public async Task<List<NotificationDto>> GetNotificationsAsync(Guid userId)
    {
        var response = await Http.GetAsync($"{ApiEndpoints.Notifications}/{userId}");
        if (!response.IsSuccessStatusCode) return [];
        return await ApiResponseReader.ReadCollectionAsync<NotificationDto>(response);
    }

    public async Task<int> GetUnreadCountAsync(Guid userId)
    {
        var response = await Http.GetAsync($"{ApiEndpoints.Notifications}/{userId}/unread-count");
        if (!response.IsSuccessStatusCode) return 0;
        var result = await ApiResponseReader.ReadAsync<UnreadCountResponse>(response);
        return result?.Count ?? 0;
    }

    public async Task SendNotificationAsync(Guid userId, string message)
    {
        try
        {
            var response = await Http.PostAsJsonAsync($"{ApiEndpoints.Notifications}/{userId}", new { message });
            if (response.IsSuccessStatusCode)
                NotificationsChanged?.Invoke(userId);
        }
        catch (HttpRequestException)
        {
        }
    }

    public async Task MarkAsReadAsync(Guid userId, Guid notificationId)
    {
        var response = await Http.PatchAsync($"{ApiEndpoints.Notifications}/{userId}/{notificationId}/read", null);
        if (response.IsSuccessStatusCode)
            NotificationsChanged?.Invoke(userId);
    }

    public async Task MarkAllAsReadAsync(Guid userId)
    {
        var response = await Http.PatchAsync($"{ApiEndpoints.Notifications}/{userId}/read-all", null);
        if (response.IsSuccessStatusCode)
            NotificationsChanged?.Invoke(userId);
    }

    private class UnreadCountResponse
    {
        public int Count { get; set; }
    }
}

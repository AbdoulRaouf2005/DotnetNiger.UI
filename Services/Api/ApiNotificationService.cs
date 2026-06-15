using System.Net.Http.Json;
using DotnetNiger.UI.Models.Responses;
using DotnetNiger.UI.Services.Contracts;

namespace DotnetNiger.UI.Services.Api;

public class ApiNotificationService : INotificationService
{
    private readonly HttpClient _http;
    private const string Base = "api/v1/notifications";

    public ApiNotificationService(HttpClient http) => _http = http;

    public event Action<Guid>? NotificationsChanged;

    public async Task<List<NotificationDto>> GetNotificationsAsync(Guid userId)
    {
        var response = await _http.GetAsync($"{Base}/{userId}");
        if (!response.IsSuccessStatusCode) return [];
        return await ApiResponseReader.ReadCollectionAsync<NotificationDto>(response);
    }

    public async Task<int> GetUnreadCountAsync(Guid userId)
    {
        var response = await _http.GetAsync($"{Base}/{userId}/unread-count");
        if (!response.IsSuccessStatusCode) return 0;
        var result = await ApiResponseReader.ReadAsync<UnreadCountResponse>(response);
        return result?.Count ?? 0;
    }

    public async Task SendNotificationAsync(Guid userId, string message)
    {
        try
        {
            var response = await _http.PostAsJsonAsync($"{Base}/{userId}", new { message });
            if (response.IsSuccessStatusCode)
                NotificationsChanged?.Invoke(userId);
        }
        catch (HttpRequestException)
        {
        }
    }

    public async Task MarkAsReadAsync(Guid userId, Guid notificationId)
    {
        var response = await _http.PatchAsync($"{Base}/{userId}/{notificationId}/read", null);
        if (response.IsSuccessStatusCode)
            NotificationsChanged?.Invoke(userId);
    }

    public async Task MarkAllAsReadAsync(Guid userId)
    {
        var response = await _http.PatchAsync($"{Base}/{userId}/read-all", null);
        if (response.IsSuccessStatusCode)
            NotificationsChanged?.Invoke(userId);
    }

    private class UnreadCountResponse
    {
        public int Count { get; set; }
    }
}

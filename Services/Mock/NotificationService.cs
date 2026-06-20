using DotnetNiger.UI.Models.Responses;
using DotnetNiger.UI.Services.Contracts;

namespace DotnetNiger.UI.Services.Mock;

public class NotificationService : INotificationService
{
    // En mémoire : userId -> notifications
    private readonly Dictionary<Guid, List<NotificationDto>> _store = new();

    public event Action<Guid>? NotificationsChanged;

    public Task SendNotificationAsync(Guid userId, string message)
    {
        if (!_store.TryGetValue(userId, out var list))
        {
            list = new List<NotificationDto>();
            _store[userId] = list;
        }

        list.Add(new NotificationDto { Message = message, CreatedAt = DateTime.UtcNow, IsRead = false });
        NotificationsChanged?.Invoke(userId);
        return Task.CompletedTask;
    }

    public async Task<List<NotificationDto>> GetNotificationsAsync(Guid userId)
    {
        await Task.Delay(800);
        if (!_store.TryGetValue(userId, out var list))
            return new List<NotificationDto>();

        return list.OrderByDescending(n => n.CreatedAt).ToList();
    }

    public async Task<int> GetUnreadCountAsync(Guid userId)
    {
        await Task.Delay(800);
        if (!_store.TryGetValue(userId, out var list))
            return 0;

        return list.Count(n => !n.IsRead);
    }

    public Task MarkAsReadAsync(Guid userId, Guid notificationId)
    {
        if (!_store.TryGetValue(userId, out var list))
            return Task.CompletedTask;

        var notification = list.FirstOrDefault(item => item.Id == notificationId);
        if (notification is null || notification.IsRead)
            return Task.CompletedTask;

        notification.IsRead = true;
        NotificationsChanged?.Invoke(userId);
        return Task.CompletedTask;
    }

    public Task MarkAllAsReadAsync(Guid userId)
    {
        if (!_store.TryGetValue(userId, out var list))
            return Task.CompletedTask;

        var changed = false;

        foreach (var notification in list.Where(item => !item.IsRead))
        {
            notification.IsRead = true;
            changed = true;
        }

        if (changed)
        {
            NotificationsChanged?.Invoke(userId);
        }

        return Task.CompletedTask;
    }
}

using DotnetNiger.UI.Services.Contracts;

namespace DotnetNiger.UI.Services.Mock;

public class ToastService : IToastService
{
    public event Action<ToastMessage>? OnShow;

    public void ShowToast(string message, ToastLevel level = ToastLevel.Info)
    {
        OnShow?.Invoke(new ToastMessage
        {
            Message = message,
            Level = level,
            Timestamp = DateTime.UtcNow
        });
    }
}

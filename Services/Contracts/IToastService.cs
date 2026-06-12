namespace DotnetNiger.UI.Services.Contracts;

public enum ToastLevel
{
    Info,
    Success,
    Warning,
    Error
}

public class ToastMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Message { get; set; } = string.Empty;
    public ToastLevel Level { get; set; } = ToastLevel.Info;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public interface IToastService
{
    event Action<ToastMessage>? OnShow;
    void ShowToast(string message, ToastLevel level = ToastLevel.Info);
}

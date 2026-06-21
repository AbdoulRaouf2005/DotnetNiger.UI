namespace DotnetNiger.UI.Models.Responses;

public class ApiSuccessResponse<T>
{
    public bool Success { get; init; } = true;
    public string? Message { get; init; }
    public T? Data { get; init; }
}

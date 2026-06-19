using System.Text.Json;
using DotnetNiger.UI.Services.Contracts;
using Microsoft.JSInterop;

namespace DotnetNiger.UI.Services.Browser;

public class JsSessionStorageService : ISessionStorageService
{
    private readonly IJSRuntime _jsRuntime;

    public JsSessionStorageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<T?> GetItemAsync<T>(string key)
    {
        var json = await _jsRuntime.InvokeAsync<string?>("sessionStorage.getItem", key);

        if (string.IsNullOrWhiteSpace(json))
            return default;

        return JsonSerializer.Deserialize<T>(json);
    }

    public async Task SetItemAsync<T>(string key, T value)
    {
        var json = JsonSerializer.Serialize(value);
        await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", key, json);
    }

    public Task RemoveItemAsync(string key)
        => _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", key).AsTask();
}

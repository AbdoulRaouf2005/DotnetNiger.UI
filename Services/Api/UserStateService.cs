using DotnetNiger.UI.Models.Responses;
using DotnetNiger.UI.Services.Contracts;

namespace DotnetNiger.UI.Services.Api;

public class UserStateService : IUserStateService
{
    public event Action? OnChange;

    private UserDto? _currentUser;

    public UserDto? CurrentUser => _currentUser;
    public bool IsAuthenticated => _currentUser is { IsActive: true };
    public Guid UserId => _currentUser?.Id ?? Guid.Empty;
    public string UserName => _currentUser?.FullName ?? string.Empty;
    public bool IsAdmin => _currentUser?.Roles.Contains("Admin") ?? false;
    public string? UserRole => _currentUser?.Roles.FirstOrDefault();

    public Task LoadUserFromStorageAsync()
    {
        return Task.CompletedTask;
    }

    public Task SetUserAsync(UserDto user)
    {
        _currentUser = user;
        OnChange?.Invoke();
        return Task.CompletedTask;
    }

    public Task UpdateUserAsync(UserDto updatedUser)
    {
        if (_currentUser is not null && _currentUser.Id == updatedUser.Id)
        {
            _currentUser = updatedUser;
            OnChange?.Invoke();
        }
        return Task.CompletedTask;
    }

    public Task ClearUserAsync()
    {
        _currentUser = null;
        OnChange?.Invoke();
        return Task.CompletedTask;
    }

    public async Task RefreshUserAsync()
    {
        await LoadUserFromStorageAsync();
    }
}

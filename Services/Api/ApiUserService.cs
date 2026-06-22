using System.Net.Http.Json;
using DotnetNiger.UI.Helpers;
using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Models.Responses;
using DotnetNiger.UI.Services.Contracts;

namespace DotnetNiger.UI.Services.Api;

public class ApiUserService : ApiServiceBase, IUserService
{
    public ApiUserService(HttpClient http) : base(http)
    {
    }

    public async Task<List<UserDto>> GetUsersAsync()
    {
        var response = await Http.GetAsync(ApiEndpoints.AdminUsers);
        if (!response.IsSuccessStatusCode)
            return [];

        return await ApiResponseReader.ReadCollectionAsync<UserDto>(response);
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid userId)
    {
        var response = await Http.GetAsync($"{ApiEndpoints.AdminUsers}/{userId}");
        if (!response.IsSuccessStatusCode)
            return null;

        return await ApiResponseReader.ReadAsync<UserDto>(response);
    }

    public async Task<List<UserDto>> GetPendingUsersAsync()
    {
        var users = await GetUsersAsync();
        return users.Where(u => !u.IsActive).ToList();
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email)
    {
        var users = await GetUsersAsync();
        return users.FirstOrDefault(u =>
            u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<List<UserDto>> SearchUsersAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return await GetUsersAsync();

        var users = await GetUsersAsync();
        var q = query.Trim();
        return users.Where(u =>
            u.Username.Contains(q, StringComparison.OrdinalIgnoreCase) ||
            u.FullName.Contains(q, StringComparison.OrdinalIgnoreCase) ||
            u.Email.Contains(q, StringComparison.OrdinalIgnoreCase) ||
            u.Country.Contains(q, StringComparison.OrdinalIgnoreCase) ||
            u.City.Contains(q, StringComparison.OrdinalIgnoreCase) ||
            u.Roles.Any(r => r.Contains(q, StringComparison.OrdinalIgnoreCase))
        ).ToList();
    }

    public async Task<List<UserDto>> GetUsersByRoleAsync(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
            return await GetUsersAsync();

        var users = await GetUsersAsync();
        return users.Where(u =>
            u.Roles.Any(r => r.Equals(role, StringComparison.OrdinalIgnoreCase))
        ).ToList();
    }

    public async Task<int> GetUsersCountAsync()
    {
        var users = await GetUsersAsync();
        return users.Count;
    }

    public async Task<int> GetActiveUsersCountAsync()
    {
        var users = await GetUsersAsync();
        return users.Count(u => u.IsActive);
    }

    public Task<UserDto> CreateUserAsync(CreateUserRequest user)
    {
        throw new NotSupportedException("La création d'utilisateur via l'API Community n'est pas implémentée. Utilisez le flux d'inscription (Register).");
    }

    public async Task<UserDto?> UpdateUserAsync(UserDto user)
    {
        var statusContent = JsonContent.Create(new UpdateUserStatusRequest { IsActive = user.IsActive });
        var statusResponse = await Http.PatchAsync($"{ApiEndpoints.AdminUsers}/{user.Id}/status", statusContent);
        if (!statusResponse.IsSuccessStatusCode)
            return null;

        var existing = await GetUserByIdAsync(user.Id);
        if (existing is null) return null;

        foreach (var role in user.Roles)
        {
            if (!existing.Roles.Contains(role, StringComparer.OrdinalIgnoreCase))
            {
                var roleContent = JsonContent.Create(new { roleName = role });
                await Http.PostAsync($"{ApiEndpoints.AdminUsers}/{user.Id}/roles", roleContent);
            }
        }

        return await GetUserByIdAsync(user.Id);
    }

    public Task<bool> DeleteUserAsync(Guid userId)
    {
        throw new NotSupportedException("La suppression d'utilisateur via l'API Community n'est pas implémentée.");
    }

    public async Task<bool> ApproveUserAsync(Guid userId)
    {
        var statusContent = JsonContent.Create(new UpdateUserStatusRequest { IsActive = true });
        var response = await Http.PatchAsync($"{ApiEndpoints.AdminUsers}/{userId}/status", statusContent);
        if (!response.IsSuccessStatusCode)
            return false;

        var roleContent = JsonContent.Create(new { roleName = RoleConstants.Member });
        await Http.PostAsync($"{ApiEndpoints.AdminUsers}/{userId}/roles", roleContent);

        return true;
    }

    public async Task<bool> RejectUserAsync(Guid userId)
    {
        var content = JsonContent.Create(new UpdateUserStatusRequest { IsActive = false });
        var response = await Http.PatchAsync($"{ApiEndpoints.AdminUsers}/{userId}/status", content);
        return response.IsSuccessStatusCode;
    }
}

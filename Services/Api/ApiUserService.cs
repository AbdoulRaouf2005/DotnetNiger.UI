using System.Net.Http.Json;
using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Models.Responses;
using DotnetNiger.UI.Services.Contracts;

namespace DotnetNiger.UI.Services.Api;

public class ApiUserService : IUserService
{
    private readonly HttpClient _http;
    private const string AdminUsersBase = "api/v1/admin/users";

    public ApiUserService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<UserDto>> GetUsersAsync()
    {
        var response = await _http.GetAsync(AdminUsersBase);
        if (!response.IsSuccessStatusCode)
            return new List<UserDto>();

        return await ApiResponseReader.ReadCollectionAsync<UserDto>(response);
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid userId)
    {
        var response = await _http.GetAsync($"{AdminUsersBase}/{userId}");
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
        throw new NotSupportedException("La création d'utilisateur via l'API Community n'est pas encore implémentée.");
    }

    public async Task<UserDto?> UpdateUserAsync(UserDto user)
    {
        var content = JsonContent.Create(new { isActive = user.IsActive });
        var response = await _http.PatchAsync($"{AdminUsersBase}/{user.Id}/status", content);
        if (!response.IsSuccessStatusCode)
            return null;

        return await GetUserByIdAsync(user.Id);
    }

    public Task<bool> DeleteUserAsync(Guid userId)
    {
        throw new NotSupportedException("La suppression d'utilisateur via l'API Community n'est pas encore implémentée.");
    }

    public Task<bool> ApproveUserAsync(Guid userId)
    {
        throw new NotSupportedException("L'approbation d'utilisateur via l'API Community n'est pas encore implémentée.");
    }

    public Task<bool> RejectUserAsync(Guid userId)
    {
        throw new NotSupportedException("Le rejet d'utilisateur via l'API Community n'est pas encore implémentée.");
    }
}

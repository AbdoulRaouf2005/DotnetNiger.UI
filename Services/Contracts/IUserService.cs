using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Models.Responses;

namespace DotnetNiger.UI.Services.Contracts;

public interface IUserService
{
    Task<UserDto?> GetUserByIdAsync(Guid userId);
    Task<List<UserDto>> GetUsersAsync();
    Task<List<UserDto>> GetPendingUsersAsync();
    Task<UserDto?> GetUserByEmailAsync(string email);
    Task<List<UserDto>> SearchUsersAsync(string query);
    Task<List<UserDto>> GetUsersByRoleAsync(string role);
    Task<int> GetUsersCountAsync();
    Task<int> GetActiveUsersCountAsync();
    Task<UserDto> CreateUserAsync(CreateUserRequest user);
    Task<UserDto?> UpdateUserAsync(UserDto user);
    Task<bool> DeleteUserAsync(Guid userId);
    Task<bool> ApproveUserAsync(Guid userId);
    Task<bool> RejectUserAsync(Guid userId);
}
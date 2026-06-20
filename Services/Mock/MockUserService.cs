using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Models.Responses;
using DotnetNiger.UI.Services.Contracts;

namespace DotnetNiger.UI.Services.Mock;

public class MockUserService : IUserService
{
    private static readonly List<UserDto> Users = new()
    {
        new UserDto
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Username = "admin",
            Email = "admin@dotnet.niger",
            FullName = "Ali Mahamane",
            Country = "Niger",
            City = "Niamey",
            IsActive = true,
            CreatedAt = DateTime.Now.AddMonths(-6),
            LastLoginAt = DateTime.Now.AddDays(-1),
            Roles = new List<string> { "Admin", "Member" },
            Skills = new List<string> { "C#", "ASP.NET Core", "Blazor", "Azure" }
        },
        new UserDto
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Username = "fatima",
            Email = "fatima@dotnet.niger",
            FullName = "Fatima Oumar",
            Country = "Niger",
            City = "Niamey",
            IsActive = true,
            CreatedAt = DateTime.Now.AddMonths(-3),
            LastLoginAt = DateTime.Now.AddDays(-5),
            Roles = new List<string> { "Member" },
            Skills = new List<string> { "C#", "ASP.NET Core", "Entity Framework" }
        },
        new UserDto
        {
            Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            Username = "moussa",
            Email = "moussa@dotnet.niger",
            FullName = "Moussa Issa",
            Country = "Niger",
            City = "Maradi",
            IsActive = false,
            CreatedAt = DateTime.Now.AddMonths(-1),
            Roles = new List<string> { "Member" },
            Skills = new List<string> { "C#", "Blazor" }
        }
    };

    public async Task<UserDto?> GetUserByIdAsync(Guid userId)
    {
        await Task.Delay(800);
        return Users.FirstOrDefault(user => user.Id == userId);
    }

    public async Task<List<UserDto>> GetUsersAsync()
    {
        await Task.Delay(800);
        return Users.ToList();
    }

    public async Task<List<UserDto>> GetPendingUsersAsync()
    {
        await Task.Delay(800);
        return Users.Where(user => !user.IsActive).ToList();
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email)
    {
        await Task.Delay(800);
        return Users.FirstOrDefault(user => user.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<List<UserDto>> SearchUsersAsync(string query)
    {
        await Task.Delay(800);
        if (string.IsNullOrWhiteSpace(query))
            return Users.ToList();

        var normalizedQuery = query.Trim();

        var filteredUsers = Users.Where(user =>
                user.Username.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) ||
                user.FullName.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) ||
                user.Email.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) ||
                user.Country.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) ||
                user.City.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) ||
                user.Roles.Any(role => role.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        return filteredUsers;
    }

    public async Task<List<UserDto>> GetUsersByRoleAsync(string role)
    {
        await Task.Delay(800);
        if (string.IsNullOrWhiteSpace(role))
            return Users.ToList();

        var filteredUsers = Users
            .Where(user => user.Roles.Any(userRole => userRole.Equals(role, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        return filteredUsers;
    }

    public async Task<int> GetUsersCountAsync()
    {
        await Task.Delay(800);
        return Users.Count;
    }

    public async Task<int> GetActiveUsersCountAsync()
    {
        await Task.Delay(800);
        return Users.Count(user => user.IsActive);
    }

    public Task<UserDto> CreateUserAsync(CreateUserRequest user)
    {
        var newUser = new UserDto
        {
            Id = Guid.NewGuid(),
            Username = string.Empty,
            Email = user.Email,
            FullName = user.FullName,
            PhoneNumber = string.Empty,
            Bio = string.Empty,
            AvatarUrl = string.Empty,
            Country = string.Empty,
            City = string.Empty,
            IsActive = true,
            CreatedAt = DateTime.Now ,
            LastLoginAt = DateTime.Now,
            Skills = new List<string>(),
            Roles = new List<string> { "Member" },
            SocialLinks = new List<SocialLinkDto>()
        };

        Users.Add(newUser);
        return Task.FromResult(newUser);
    }

    public Task<UserDto?> UpdateUserAsync(UserDto user)
    {
        var existing = Users.FirstOrDefault(item => item.Id == user.Id);
        if (existing is null)
            return Task.FromResult<UserDto?>(null);

        existing.Username = string.IsNullOrWhiteSpace(user.Username) ? existing.Username : user.Username;
        existing.Email = string.IsNullOrWhiteSpace(user.Email) ? existing.Email : user.Email;
        existing.FullName = string.IsNullOrWhiteSpace(user.FullName) ? existing.FullName : user.FullName;
        existing.PhoneNumber = user.PhoneNumber;
        existing.Bio = user.Bio;
        existing.AvatarUrl = string.IsNullOrWhiteSpace(user.AvatarUrl) ? existing.AvatarUrl : user.AvatarUrl;
        existing.Country = user.Country;
        existing.City = user.City;
        existing.IsActive = user.IsActive;
        existing.Skills = user.Skills.Count > 0 ? user.Skills.ToList() : existing.Skills;
        existing.Roles = user.Roles.Count > 0 ? user.Roles.ToList() : existing.Roles;
        existing.SocialLinks = user.SocialLinks.Count > 0 ? user.SocialLinks.ToList() : existing.SocialLinks;

        return Task.FromResult<UserDto?>(existing);
    }

    public Task<bool> DeleteUserAsync(Guid userId)
    {
        var removed = Users.RemoveAll(user => user.Id == userId) > 0;
        return Task.FromResult(removed);
    }

    public Task<bool> ApproveUserAsync(Guid userId)
    {
        var user = Users.FirstOrDefault(item => item.Id == userId);
        if (user is null)
            return Task.FromResult(false);

        user.IsActive = true;
        if (!user.Roles.Any(role => role.Equals("Member", StringComparison.OrdinalIgnoreCase)))
        {
            user.Roles.Add("Member");
        }

        return Task.FromResult(true);
    }

    public Task<bool> RejectUserAsync(Guid userId)
    {
        var removed = Users.RemoveAll(user => user.Id == userId && !user.IsActive) > 0;
        return Task.FromResult(removed);
    }
}
using DotnetNiger.UI.Models.Responses;

namespace DotnetNiger.UI.Services.Helpers;

public static class MockDataStore
{
    public static readonly List<UserDto> Users = new()
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
            IsActive = true,
            CreatedAt = DateTime.Now.AddMonths(-1),
            Roles = new List<string> { "Member" },
            Skills = new List<string> { "C#", "Blazor" }
        }
    };
}

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
            Bio = "Passionné par l'écosystème .NET depuis plus de 8 ans. Je crois en la force de la communauté pour transformer les défis techniques en opportunités d'innovation.",
            Country = "Niger",
            City = "Niamey",
            IsActive = true,
            IsTeamMember = true,
            Position = "Lead Organisateur",
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
            Bio = "Community Manager passionnée par le développement web et la création de ponts entre les développeurs nigériens. Organisatrice de meetups et d'ateliers techniques.",
            Country = "Niger",
            City = "Niamey",
            IsActive = true,
            IsTeamMember = true,
            Position = "Community Manager",
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
            Bio = "Développeur full-stack basé à Maradi, spécialisé dans les applications web modernes avec Blazor et ASP.NET Core. Contribue activement aux projets open-source de la communauté.",
            Country = "Niger",
            City = "Maradi",
            IsActive = true,
            IsTeamMember = true,
            Position = "Développeur Full-Stack",
            CreatedAt = DateTime.Now.AddMonths(-1),
            Roles = new List<string> { "Member" },
            Skills = new List<string> { "C#", "Blazor" }
        }
    };
}

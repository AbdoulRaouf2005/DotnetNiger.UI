using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Models.Responses;
using DotnetNiger.UI.Services.Contracts;

namespace DotnetNiger.UI.Services.Mock;

public class ResourceService : IResourceService
{
    private List<ResourceDto> _resources;

    public ResourceService()
    {
        _resources = new List<ResourceDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Documentation officielle .NET 8",
                Slug = "documentation-officielle-dotnet-8",
                Description = "La documentation complète de .NET 8 par Microsoft : guides, tutoriels et référence API.",
                Url = "https://learn.microsoft.com/fr-fr/dotnet/",
                ResourceType = "Documentation",
                Level = "Tous niveaux",
                ViewCount = 320,
                CreatedAt = new DateTime(2025, 6, 15),
                Tags = [new TagDto { Id = Guid.NewGuid(), Name = "dotnet", Slug = "dotnet", UsageCount = 45 }]
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Tutoriel Blazor WebAssembly — Getting Started",
                Slug = "tutoriel-blazor-webassembly-getting-started",
                Description = "Guide de démarrage officiel Microsoft pour créer votre première application Blazor WASM.",
                Url = "https://learn.microsoft.com/fr-fr/aspnet/core/blazor/",
                ResourceType = "Tutoriel",
                Level = "Débutant",
                ViewCount = 214,
                CreatedAt = new DateTime(2025, 7, 2),
                Tags = [new TagDto { Id = Guid.NewGuid(), Name = "blazor", Slug = "blazor", UsageCount = 38 }]
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Clean Architecture avec ASP.NET Core",
                Slug = "clean-architecture-aspnet-core",
                Description = "Implémentation de la Clean Architecture dans un projet ASP.NET Core : séparation des couches, DI, CQRS.",
                Url = "https://github.com/ardalis/CleanArchitecture",
                ResourceType = "GitHub",
                Level = "Intermédiaire",
                ViewCount = 178,
                CreatedAt = new DateTime(2025, 8, 10),
                Tags =
                [
                    new TagDto { Id = Guid.NewGuid(), Name = "architecture", Slug = "architecture", UsageCount = 52 },
                    new TagDto { Id = Guid.NewGuid(), Name = "clean-code", Slug = "clean-code", UsageCount = 31 }
                ]
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Design Patterns en C# — Refactoring.Guru",
                Slug = "design-patterns-csharp-refactoring-guru",
                Description = "Catalogue complet des design patterns GoF avec exemples en C# et diagrammes UML.",
                Url = "https://refactoring.guru/fr/design-patterns/csharp",
                ResourceType = "Article",
                Level = "Avancé",
                ViewCount = 95,
                CreatedAt = new DateTime(2025, 9, 5),
                Tags = [new TagDto { Id = Guid.NewGuid(), Name = "design-patterns", Slug = "design-patterns", UsageCount = 27 }]
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Entity Framework Core — Guide complet",
                Slug = "entity-framework-core-guide-complet",
                Description = "Tout ce qu'il faut savoir sur EF Core : migrations, requêtes LINQ, relations, performance.",
                Url = "https://learn.microsoft.com/fr-fr/ef/core/",
                ResourceType = "Documentation",
                Level = "Intermédiaire",
                ViewCount = 267,
                CreatedAt = new DateTime(2025, 7, 20),
                Tags =
                [
                    new TagDto { Id = Guid.NewGuid(), Name = "ef-core", Slug = "ef-core", UsageCount = 41 },
                    new TagDto { Id = Guid.NewGuid(), Name = "database", Slug = "database", UsageCount = 35 }
                ]
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Introduction à SignalR pour le temps réel",
                Slug = "introduction-signalr-temps-reel",
                Description = "Apprenez à utiliser SignalR pour ajouter des fonctionnalités temps réel à vos applications ASP.NET Core.",
                Url = "https://learn.microsoft.com/fr-fr/aspnet/signalr/",
                ResourceType = "Tutoriel",
                Level = "Intermédiaire",
                ViewCount = 143,
                CreatedAt = new DateTime(2025, 10, 1)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Projet .NET Niger — API Gateway Ocelot",
                Slug = "projet-dotnet-niger-api-gateway-ocelot",
                Description = "Architecture et mise en Å“uvre de la passerelle API du projet DotNet Niger avec Ocelot et Consul.",
                Url = "https://github.com/dotnetniger/platform",
                ResourceType = "GitHub",
                Level = "Avancé",
                ViewCount = 67,
                CreatedAt = new DateTime(2025, 11, 12)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Programmation asynchrone en C#",
                Slug = "programmation-asynchrone-csharp",
                Description = "Maîtrisez async/await, Task, ValueTask et le parallélisme en C# avec des exemples concrets.",
                Url = "https://learn.microsoft.com/fr-fr/dotnet/csharp/asynchronous-programming/",
                ResourceType = "Documentation",
                Level = "Intermédiaire",
                ViewCount = 198,
                CreatedAt = new DateTime(2025, 8, 25)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Sécuriser une API ASP.NET Core avec JWT",
                Slug = "securiser-api-aspnet-core-jwt",
                Description = "Guide pas à pas pour implémenter l'authentification JWT dans une API REST ASP.NET Core.",
                Url = "https://learn.microsoft.com/fr-fr/aspnet/core/security/",
                ResourceType = "Tutoriel",
                Level = "Intermédiaire",
                ViewCount = 156,
                CreatedAt = new DateTime(2025, 9, 18)
            }
        };
    }

    public async Task<List<ResourceDto>> GetAllResourcesAsync()
    {
        return await Task.FromResult(
            _resources.OrderByDescending(r => r.CreatedAt).ToList());
    }

    public async Task<ResourceDto?> GetResourceByIdAsync(Guid id)
    {
        return await Task.FromResult(_resources.FirstOrDefault(r => r.Id == id));
    }

    public async Task<ResourceDto?> GetResourceBySlugAsync(string slug)
    {
        var resource = _resources.FirstOrDefault(r => r.Slug == slug);
        return await Task.FromResult(resource);
    }

    public async Task<List<ResourceDto>> GetResourcesByTypeAsync(string resourceType)
    {
        return await Task.FromResult(
            _resources.Where(r => r.ResourceType.Equals(resourceType, StringComparison.OrdinalIgnoreCase))
                      .OrderByDescending(r => r.ViewCount)
                      .ToList());
    }

    public async Task<List<ResourceDto>> GetResourcesByLevelAsync(string level)
    {
        return await Task.FromResult(
            _resources.Where(r => r.Level.Equals(level, StringComparison.OrdinalIgnoreCase))
                      .OrderByDescending(r => r.ViewCount)
                      .ToList());
    }

    public async Task<List<ResourceDto>> SearchResourcesAsync(string query)
    {
        return await Task.FromResult(
            _resources.Where(r =>
                    r.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    r.Description.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    r.ResourceType.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    r.Tags.Any(t => t.Name.Contains(query, StringComparison.OrdinalIgnoreCase)))
                .OrderByDescending(r => r.ViewCount)
                .ToList());
    }

    public async Task<List<string>> GetResourceTypesAsync()
    {
        return await Task.FromResult(
            _resources.Select(r => r.ResourceType).Distinct().OrderBy(t => t).ToList());
    }

    public async Task<List<string>> GetLevelsAsync()
    {
        return await Task.FromResult(
            _resources.Select(r => r.Level).Distinct().OrderBy(l => l switch
            {
                "Débutant" => 0,
                "Intermédiaire" => 1,
                "Avancé" => 2,
                "Tous niveaux" => 3,
                _ => 4
            }).ToList());
    }

    public async Task<ResourceDto?> CreateResourceAsync(CreateResourceRequest request)
    {
        var newResource = new ResourceDto
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Slug = GenerateSlug(request.Title),
            Description = request.Description,
            Url = request.Url,
            ResourceType = request.ResourceType,
            Level = request.Level,
            ViewCount = 0,
            CreatedAt = DateTime.UtcNow,
            Tags = []
        };

        _resources.Add(newResource);
        return await Task.FromResult(newResource);
    }

    public async Task<ResourceDto?> AddResourceAsync(AddResourceRequest request)
    {
        var newResource = new ResourceDto
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Slug = GenerateSlug(request.Title),
            Description = request.Description,
            Url = request.Url,
            ResourceType = request.ResourceType,
            Level = request.Level,
            ViewCount = 0,
            CreatedAt = DateTime.UtcNow,
            Tags = []
        };

        _resources.Add(newResource);
        return await Task.FromResult(newResource);
    }

    public async Task<ResourceDto?> UpdateResourceAsync(Guid id, CreateResourceRequest request)
    {
        var resource = _resources.FirstOrDefault(r => r.Id == id);
        if (resource is null) return await Task.FromResult<ResourceDto?>(null);

        resource.Title = request.Title;
        resource.Slug = GenerateSlug(request.Title);
        resource.Description = request.Description;
        resource.Url = request.Url;
        resource.ResourceType = request.ResourceType;
        resource.Level = request.Level;

        return await Task.FromResult<ResourceDto?>(resource);
    }

    public async Task<bool> DeleteResourceAsync(Guid id)
    {
        var resource = _resources.FirstOrDefault(r => r.Id == id);
        if (resource is null) return await Task.FromResult(false);
        _resources.Remove(resource);
        return await Task.FromResult(true);
    }

    public async Task IncrementViewCountAsync(Guid id)
    {
        var resource = _resources.FirstOrDefault(r => r.Id == id);
        if (resource is not null) resource.ViewCount++;
        await Task.CompletedTask;
    }

    private static string GenerateSlug(string title)
    {
        return title
            .ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("à", "a").Replace("â", "a")
            .Replace("é", "e").Replace("è", "e").Replace("ê", "e").Replace("ë", "e")
            .Replace("î", "i").Replace("ï", "i")
            .Replace("ô", "o")
            .Replace("ù", "u").Replace("û", "u")
            .Replace("ç", "c")
            .Replace("'", "-").Replace("\"", "")
            .Replace(",", "").Replace(".", "")
            .Replace("?", "").Replace("!", "").Replace("#", "");
    }
}

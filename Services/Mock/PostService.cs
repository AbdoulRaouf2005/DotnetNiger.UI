using DotnetNiger.UI.Models.Requests;
using DotnetNiger.UI.Models.Responses;
using DotnetNiger.UI.Services.Contracts;

namespace DotnetNiger.UI.Services.Mock
{
    public class PostService : IPostService
    {
        private List<PostDto> _posts;

        public PostService()
        {
            _posts = new List<PostDto>
            {
                new PostDto
                {
                    Id = Guid.NewGuid(),
                    Title = "Les nouveaut�s de .NET 9",
                    Slug = "les-nouveautes-de-dotnet-9",
                    Excerpt = "Découvrez les dernières fonctionnalités et améliorations de .NET 9, avec C# 13 comme langage phare.",
                    Content = "<h1>Introduction</h1><p>...</p>",
                    CoverImageUrl = "/images/dotnet9.jpg",
                    AuthorId = Guid.NewGuid(),
                    AuthorName = "Jean Dupont",
                    AuthorAvatar = "/Images/ImageBlog.jpg",
                    PostType = "Article",
                    PublishedAt = DateTime.Now.AddDays(-5),
                    ViewCount = 245,
                    Categories = new List<CategoryDto>
                    {
                        new CategoryDto { Id = Guid.NewGuid(), Name = "Technologie", Slug = "technologie", Description = "", PostCount = 10 }
                    },
                    Tags = new List<TagDto>
                    {
                        new TagDto { Id = Guid.NewGuid(), Name = ".NET9", Slug = "dotnet9", PostCount = 5 },
                        new TagDto { Id = Guid.NewGuid(), Name = "C#", Slug = "csharp", PostCount = 15 }
                    }
                },
                new PostDto
                {
                    Id = Guid.NewGuid(),
                    Title = "Introduction � Blazor WebAssembly",
                    Slug = "introduction-a-blazor-webassembly",
                    Excerpt = "Apprenez les bases de Blazor WASM...",
                    Content = "<h1>Blazor WASM</h1><p>...</p>",
                    CoverImageUrl = "/images/blazor.jpg",
                    AuthorId = Guid.NewGuid(),
                    AuthorName = "Marie Martin",
                    AuthorAvatar = "/images/avatars/marie.jpg",
                    PostType = "Tutorial",
                    PublishedAt = DateTime.Now.AddDays(-10),
                    ViewCount = 512,
                    Categories = new List<CategoryDto>
                    {
                        new CategoryDto { Id = Guid.NewGuid(), Name = "Web", Slug = "web", Description = "", PostCount = 20 }
                    },
                    Tags = new List<TagDto>
                    {
                        new TagDto { Id = Guid.NewGuid(), Name = "Blazor", Slug = "blazor", PostCount = 8 },
                        new TagDto { Id = Guid.NewGuid(), Name = "WebAssembly", Slug = "webassembly", PostCount = 6 }
                    }
                },
                new PostDto
                {
                    Id = Guid.NewGuid(),
                    Title = "Les meilleures pratiques en C#",
                    Slug = "les-meilleures-pratiques-en-csharp",
                    Excerpt = "Guide complet des meilleures pratiques en C# : nommage, SOLID, gestion des erreurs.",
                    Content = "<h2>Nommage significatif</h2><p>Utilisez des noms clairs et expressifs.</p><h2>Principes SOLID</h2><ul><li>Single Responsibility</li><li>Open/Closed</li><li>Liskov Substitution</li><li>Interface Segregation</li><li>Dependency Inversion</li></ul><h2>Gestion des erreurs</h2><p>Utilisez try-catch et using pour la gestion des ressources.</p>",
                    CoverImageUrl = "/images/csharp.jpg",
                    AuthorId = Guid.NewGuid(),
                    AuthorName = "Pierre Dubois",
                    AuthorAvatar = "/images/avatars/pierre.jpg",
                    PostType = "Article",
                    PublishedAt = DateTime.Now.AddDays(-3),
                    ViewCount = 389,
                    Categories = new List<CategoryDto>
                    {
                        new CategoryDto { Id = Guid.NewGuid(), Name = "Programmation", Slug = "programmation", Description = "", PostCount = 35 }
                    },
                    Tags = new List<TagDto>
                    {
                        new TagDto { Id = Guid.NewGuid(), Name = "C#", Slug = "csharp", PostCount = 15 },
                        new TagDto { Id = Guid.NewGuid(), Name = "Best Practices", Slug = "best-practices", PostCount = 12 }
                    }
                },
                new PostDto
                {
                    Id = Guid.NewGuid(),
                    Title = "Introduction � Blazor WebAssembly",
                    Slug = "introduction-a-blazor-webassembly",
                    Excerpt = "Apprenez les bases de Blazor WebAssembly, la révolution C# pour le développement web moderne.",
                    Content = "<h2>Qu'est-ce que Blazor WebAssembly?</h2><p>Blazor WASM est un framework de frontend qui permet d'exécuter du code C# directement dans le navigateur via WebAssembly.</p><h2>Avantages</h2><ul><li>Partage de code C#</li><li>Performance optimale</li><li>Sécurité renforcée</li><li>Productivité accrue</li></ul>",
                    CoverImageUrl = "/images/blazor.jpg",
                    AuthorId = Guid.NewGuid(),
                    AuthorName = "Marie Martin",
                    AuthorAvatar = "/images/avatars/marie.jpg",
                    PostType = "Tutorial",
                    PublishedAt = DateTime.Now.AddDays(-10),
                    ViewCount = 512,
                    Categories = new List<CategoryDto>
                    {
                        new CategoryDto { Id = Guid.NewGuid(), Name = "Web", Slug = "web", Description = "", PostCount = 20 }
                    },
                    Tags = new List<TagDto>
                    {
                        new TagDto { Id = Guid.NewGuid(), Name = "Blazor", Slug = "blazor", PostCount = 8 },
                        new TagDto { Id = Guid.NewGuid(), Name = "WebAssembly", Slug = "webassembly", PostCount = 6 }
                    }
                },
                new PostDto
                {
                    Id = Guid.NewGuid(),
                    Title = "Introduction � Blazor WebAssembly",
                    Slug = "introduction-a-blazor-webassembly",
                    Excerpt = "Apprenez les bases de Blazor WASM...",
                    Content = "<h1>Blazor WASM</h1><p>...</p>",
                    CoverImageUrl = "/images/blazor.jpg",
                    AuthorId = Guid.NewGuid(),
                    AuthorName = "Marie Martin",
                    AuthorAvatar = "/images/avatars/marie.jpg",
                    PostType = "Tutorial",
                    PublishedAt = DateTime.Now.AddDays(-10),
                    ViewCount = 512,
                    Categories = new List<CategoryDto>
                    {
                        new CategoryDto { Id = Guid.NewGuid(), Name = "Web", Slug = "web", Description = "", PostCount = 20 }
                    },
                    Tags = new List<TagDto>
                    {
                        new TagDto { Id = Guid.NewGuid(), Name = "Blazor", Slug = "blazor", PostCount = 8 },
                        new TagDto { Id = Guid.NewGuid(), Name = "WebAssembly", Slug = "webassembly", PostCount = 6 }
                    }
                },
                new PostDto
                {
                    Id = Guid.NewGuid(),
                    Title = "Introduction � Blazor WebAssembly",
                    Slug = "introduction-a-blazor-webassembly",
                    Excerpt = "Apprenez les bases de Blazor WASM...",
                    Content = "<h1>Blazor WASM</h1><p>...</p>",
                    CoverImageUrl = "/images/blazor.jpg",
                    AuthorId = Guid.NewGuid(),
                    AuthorName = "Marie Martin",
                    AuthorAvatar = "/images/avatars/marie.jpg",
                    PostType = "Tutorial",
                    PublishedAt = DateTime.Now.AddDays(-10),
                    ViewCount = 512,
                    Categories = new List<CategoryDto>
                    {
                        new CategoryDto { Id = Guid.NewGuid(), Name = "Web", Slug = "web", Description = "", PostCount = 20 }
                    },
                    Tags = new List<TagDto>
                    {
                        new TagDto { Id = Guid.NewGuid(), Name = "Blazor", Slug = "blazor", PostCount = 8 },
                        new TagDto { Id = Guid.NewGuid(), Name = "WebAssembly", Slug = "webassembly", PostCount = 6 }
                    }
                },
                new PostDto
                {
                    Id = Guid.NewGuid(),
                    Title = "Introduction � Blazor WebAssembly",
                    Slug = "introduction-a-blazor-webassembly",
                    Excerpt = "Apprenez les bases de Blazor WASM...",
                    Content = "<h1>Blazor WASM</h1><p>...</p>",
                    CoverImageUrl = "/images/blazor.jpg",
                    AuthorId = Guid.NewGuid(),
                    AuthorName = "Marie Martin",
                    AuthorAvatar = "/images/avatars/marie.jpg",
                    PostType = "Tutorial",
                    PublishedAt = DateTime.Now.AddDays(-10),
                    ViewCount = 512,
                    Categories = new List<CategoryDto>
                    {
                        new CategoryDto { Id = Guid.NewGuid(), Name = "Web", Slug = "web", Description = "", PostCount = 20 }
                    },
                    Tags = new List<TagDto>
                    {
                        new TagDto { Id = Guid.NewGuid(), Name = "Blazor", Slug = "blazor", PostCount = 8 },
                        new TagDto { Id = Guid.NewGuid(), Name = "WebAssembly", Slug = "webassembly", PostCount = 6 }
                    }
                },
                new PostDto
                {
                    Id = Guid.NewGuid(),
                    Title = "Introduction � Blazor WebAssembly",
                    Slug = "introduction-a-blazor-webassembly",
                    Excerpt = "Apprenez les bases de Blazor WASM...",
                    Content = "<h1>Blazor WASM</h1><p>...</p>",
                    CoverImageUrl = "/images/blazor.jpg",
                    AuthorId = Guid.NewGuid(),
                    AuthorName = "Marie Martin",
                    AuthorAvatar = "/images/avatars/marie.jpg",
                    PostType = "Tutorial",
                    PublishedAt = DateTime.Now.AddDays(-10),
                    ViewCount = 512,
                    Categories = new List<CategoryDto>
                    {
                        new CategoryDto { Id = Guid.NewGuid(), Name = "Web", Slug = "web", Description = "", PostCount = 20 }
                    },
                    Tags = new List<TagDto>
                    {
                        new TagDto { Id = Guid.NewGuid(), Name = "Blazor", Slug = "blazor", PostCount = 8 },
                        new TagDto { Id = Guid.NewGuid(), Name = "WebAssembly", Slug = "webassembly", PostCount = 6 }
                    }
                },
                new PostDto
                {
                    Id = Guid.NewGuid(),
                    Title = "Introduction � Blazor WebAssembly",
                    Slug = "introduction-a-blazor-webassembly",
                    Excerpt = "Apprenez les bases de Blazor WASM...",
                    Content = "<h1>Blazor WASM</h1><p>...</p>",
                    CoverImageUrl = "/images/blazor.jpg",
                    AuthorId = Guid.NewGuid(),
                    AuthorName = "Marie Martin",
                    AuthorAvatar = "/images/avatars/marie.jpg",
                    PostType = "Tutorial",
                    PublishedAt = DateTime.Now.AddDays(-10),
                    ViewCount = 512,
                    Categories = new List<CategoryDto>
                    {
                        new CategoryDto { Id = Guid.NewGuid(), Name = "Web", Slug = "web", Description = "", PostCount = 20 }
                    },
                    Tags = new List<TagDto>
                    {
                        new TagDto { Id = Guid.NewGuid(), Name = "Blazor", Slug = "blazor", PostCount = 8 },
                        new TagDto { Id = Guid.NewGuid(), Name = "WebAssembly", Slug = "webassembly", PostCount = 6 }
                    }
                },
                new PostDto
                {
                    Id = Guid.NewGuid(),
                    Title = "Introduction � Blazor WebAssembly",
                    Slug = "introduction-a-blazor-webassembly",
                    Excerpt = "Apprenez les bases de Blazor WASM...",
                    Content = "<h1>Blazor WASM</h1><p>...</p>",
                    CoverImageUrl = "/images/blazor.jpg",
                    AuthorId = Guid.NewGuid(),
                    AuthorName = "Marie Martin",
                    AuthorAvatar = "/images/avatars/marie.jpg",
                    PostType = "Tutorial",
                    PublishedAt = DateTime.Now.AddDays(-10),
                    ViewCount = 512,
                    Categories = new List<CategoryDto>
                    {
                        new CategoryDto { Id = Guid.NewGuid(), Name = "Web", Slug = "web", Description = "", PostCount = 20 }
                    },
                    Tags = new List<TagDto>
                    {
                        new TagDto { Id = Guid.NewGuid(), Name = "Blazor", Slug = "blazor", PostCount = 8 },
                        new TagDto { Id = Guid.NewGuid(), Name = "WebAssembly", Slug = "webassembly", PostCount = 6 }
                    }
                },
                new PostDto
                {
                    Id = Guid.NewGuid(),
                    Title = "Introduction � Blazor WebAssembly",
                    Slug = "introduction-a-blazor-webassembly",
                    Excerpt = "Apprenez les bases de Blazor WASM...",
                    Content = "<h1>Blazor WASM</h1><p>...</p>",
                    CoverImageUrl = "/images/blazor.jpg",
                    AuthorId = Guid.NewGuid(),
                    AuthorName = "Marie Martin",
                    AuthorAvatar = "/images/avatars/marie.jpg",
                    PostType = "Tutorial",
                    PublishedAt = DateTime.Now.AddDays(-10),
                    ViewCount = 512,
                    Categories = new List<CategoryDto>
                    {
                        new CategoryDto { Id = Guid.NewGuid(), Name = "Web", Slug = "web", Description = "", PostCount = 20 }
                    },
                    Tags = new List<TagDto>
                    {
                        new TagDto { Id = Guid.NewGuid(), Name = "Blazor", Slug = "blazor", PostCount = 8 },
                        new TagDto { Id = Guid.NewGuid(), Name = "WebAssembly", Slug = "webassembly", PostCount = 6 }
                    }
                },
                new PostDto
                {
                    Id = Guid.NewGuid(),
                    Title = "Introduction � Blazor WebAssembly",
                    Slug = "introduction-a-blazor-webassembly",
                    Excerpt = "Apprenez les bases de Blazor WASM...",
                    Content = "<h1>Blazor WASM</h1><p>...</p>",
                    CoverImageUrl = "/images/blazor.jpg",
                    AuthorId = Guid.NewGuid(),
                    AuthorName = "Marie Martin",
                    AuthorAvatar = "/images/avatars/marie.jpg",
                    PostType = "Tutorial",
                    PublishedAt = DateTime.Now.AddDays(-10),
                    ViewCount = 512,
                    Categories = new List<CategoryDto>
                    {
                        new CategoryDto { Id = Guid.NewGuid(), Name = "Web", Slug = "web", Description = "", PostCount = 20 }
                    },
                    Tags = new List<TagDto>
                    {
                        new TagDto { Id = Guid.NewGuid(), Name = "Blazor", Slug = "blazor", PostCount = 8 },
                        new TagDto { Id = Guid.NewGuid(), Name = "WebAssembly", Slug = "webassembly", PostCount = 6 }
                    }
                },
                new PostDto
                {
                    Id = Guid.NewGuid(),
                    Title = "Introduction � Blazor WebAssembly",
                    Slug = "introduction-a-blazor-webassembly",
                    Excerpt = "Apprenez les bases de Blazor WASM...",
                    Content = "<h1>Blazor WASM</h1><p>...</p>",
                    CoverImageUrl = "/images/blazor.jpg",
                    AuthorId = Guid.NewGuid(),
                    AuthorName = "Marie Martin",
                    AuthorAvatar = "/images/avatars/marie.jpg",
                    PostType = "Tutorial",
                    PublishedAt = DateTime.Now.AddDays(-10),
                    ViewCount = 512,
                    Categories = new List<CategoryDto>
                    {
                        new CategoryDto { Id = Guid.NewGuid(), Name = "Web", Slug = "web", Description = "", PostCount = 20 }
                    },
                    Tags = new List<TagDto>
                    {
                        new TagDto { Id = Guid.NewGuid(), Name = "Blazor", Slug = "blazor", PostCount = 8 },
                        new TagDto { Id = Guid.NewGuid(), Name = "WebAssembly", Slug = "webassembly", PostCount = 6 }
                    }
                },
                new PostDto
                {
                    Id = Guid.NewGuid(),
                    Title = "Introduction � Blazor WebAssembly",
                    Slug = "introduction-a-blazor-webassembly",
                    Excerpt = "Apprenez les bases de Blazor WASM...",
                    Content = "<h1>Blazor WASM</h1><p>...</p>",
                    CoverImageUrl = "/images/blazor.jpg",
                    AuthorId = Guid.NewGuid(),
                    AuthorName = "Marie Martin",
                    AuthorAvatar = "/images/avatars/marie.jpg",
                    PostType = "Tutorial",
                    PublishedAt = DateTime.Now.AddDays(-10),
                    ViewCount = 512,
                    Categories = new List<CategoryDto>
                    {
                        new CategoryDto { Id = Guid.NewGuid(), Name = "Web", Slug = "web", Description = "", PostCount = 20 }
                    },
                    Tags = new List<TagDto>
                    {
                        new TagDto { Id = Guid.NewGuid(), Name = "Blazor", Slug = "blazor", PostCount = 8 },
                        new TagDto { Id = Guid.NewGuid(), Name = "WebAssembly", Slug = "webassembly", PostCount = 6 }
                    }
                },
                new PostDto
                {
                    Id = Guid.NewGuid(),
                    Title = "Introduction � Blazor WebAssembly",
                    Slug = "introduction-a-blazor-webassembly",
                    Excerpt = "Apprenez les bases de Blazor WASM...",
                    Content = "<h1>Blazor WASM</h1><p>...</p>",
                    CoverImageUrl = "/images/blazor.jpg",
                    AuthorId = Guid.NewGuid(),
                    AuthorName = "Marie Martin",
                    AuthorAvatar = "/images/avatars/marie.jpg",
                    PostType = "Tutorial",
                    PublishedAt = DateTime.Now.AddDays(-10),
                    ViewCount = 512,
                    Categories = new List<CategoryDto>
                    {
                        new CategoryDto { Id = Guid.NewGuid(), Name = "Web", Slug = "web", Description = "", PostCount = 20 }
                    },
                    Tags = new List<TagDto>
                    {
                        new TagDto { Id = Guid.NewGuid(), Name = "Blazor", Slug = "blazor", PostCount = 8 },
                        new TagDto { Id = Guid.NewGuid(), Name = "WebAssembly", Slug = "webassembly", PostCount = 6 }
                    }
                },
                new PostDto
                {
                    Id = Guid.NewGuid(),
                    Title = "Introduction � Blazor WebAssembly",
                    Slug = "introduction-a-blazor-webassembly",
                    Excerpt = "Apprenez les bases de Blazor WASM...",
                    Content = "<h1>Blazor WASM</h1><p>...</p>",
                    CoverImageUrl = "/images/blazor.jpg",
                    AuthorId = Guid.NewGuid(),
                    AuthorName = "Marie Martin",
                    AuthorAvatar = "/images/avatars/marie.jpg",
                    PostType = "Tutorial",
                    PublishedAt = DateTime.Now.AddDays(-10),
                    ViewCount = 512,
                    Categories = new List<CategoryDto>
                    {
                        new CategoryDto { Id = Guid.NewGuid(), Name = "Web", Slug = "web", Description = "", PostCount = 20 }
                    },
                    Tags = new List<TagDto>
                    {
                        new TagDto { Id = Guid.NewGuid(), Name = "Blazor", Slug = "blazor", PostCount = 8 },
                        new TagDto { Id = Guid.NewGuid(), Name = "WebAssembly", Slug = "webassembly", PostCount = 6 }
                    }
                },
                new PostDto
                {
                    Id = Guid.NewGuid(),
                    Title = "Introduction � Blazor WebAssembly",
                    Slug = "introduction-a-blazor-webassembly",
                    Excerpt = "Apprenez les bases de Blazor WASM...",
                    Content = "<h1>Blazor WASM</h1><p>...</p>",
                    CoverImageUrl = "/images/blazor.jpg",
                    AuthorId = Guid.NewGuid(),
                    AuthorName = "Marie Martin",
                    AuthorAvatar = "/images/avatars/marie.jpg",
                    PostType = "Tutorial",
                    PublishedAt = DateTime.Now.AddDays(-10),
                    ViewCount = 512,
                    Categories = new List<CategoryDto>
                    {
                        new CategoryDto { Id = Guid.NewGuid(), Name = "Web", Slug = "web", Description = "", PostCount = 20 }
                    },
                    Tags = new List<TagDto>
                    {
                        new TagDto { Id = Guid.NewGuid(), Name = "Blazor", Slug = "blazor", PostCount = 8 },
                        new TagDto { Id = Guid.NewGuid(), Name = "WebAssembly", Slug = "webassembly", PostCount = 6 }
                    }
                },
                new PostDto
                {
                    Id = Guid.NewGuid(),
                    Title = "Introduction � Blazor WebAssembly",
                    Slug = "introduction-a-blazor-webassembly",
                    Excerpt = "Apprenez les bases de Blazor WASM...",
                    Content = "<h1>Blazor WASM</h1><p>...</p>",
                    CoverImageUrl = "/images/blazor.jpg",
                    AuthorId = Guid.NewGuid(),
                    AuthorName = "Marie Martin",
                    AuthorAvatar = "/images/avatars/marie.jpg",
                    PostType = "Tutorial",
                    PublishedAt = DateTime.Now.AddDays(-10),
                    ViewCount = 512,
                    Categories = new List<CategoryDto>
                    {
                        new CategoryDto { Id = Guid.NewGuid(), Name = "Web", Slug = "web", Description = "", PostCount = 20 }
                    },
                    Tags = new List<TagDto>
                    {
                        new TagDto { Id = Guid.NewGuid(), Name = "Blazor", Slug = "blazor", PostCount = 8 },
                        new TagDto { Id = Guid.NewGuid(), Name = "WebAssembly", Slug = "webassembly", PostCount = 6 }
                    }
                }
            };
        }

        public async Task<PostDto> CreatePostAsync(CreatePostRequest request)
        {
            var newPost = new PostDto
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Slug = GenerateSlug(request.Title),
                Excerpt = request.Excerpt,
                Content = request.Content,
                CoverImageUrl = request.CoverImageUrl ?? "/images/default.jpg",
                AuthorId = Guid.NewGuid(), // � remplacer par l'utilisateur connect�
                AuthorName = "Admin",
                AuthorAvatar = "/images/avatars/default.jpg",
                PostType = request.PostType,
                PublishedAt = request.IsPublished ? DateTime.Now : DateTime.MinValue,
                ViewCount = 0,
                Categories = new List<CategoryDto>(),
                Tags = new List<TagDto>(),
            };

            _posts.Add(newPost);

            return await Task.FromResult(newPost);
        }

        public async Task<bool> DeletePostAsync(Guid id)
        {
            var post = _posts.FirstOrDefault(p => p.Id == id);
            if (post == null)
                return await Task.FromResult(false);

            _posts.Remove(post);
            return await Task.FromResult(true);
        }

        public async Task<List<PostDto>> GetAllPostsAsync()
        {
            var posts = _posts
                .OrderByDescending(p => p.PublishedAt)
                .ToList();

            return await Task.FromResult(posts);
        }
  
        public async Task<List<PostDto>> GetPublishedPostsAsync()
        {
            var posts = _posts
                .Where(p => p.PublishedAt != DateTime.MinValue)
                .OrderByDescending(p => p.PublishedAt)
                .ToList();

            return await Task.FromResult(posts);
        }

        public async Task<List<PostDto>> GetPostsByCategoryAsync(string categorySlug)
        {
            var posts = _posts
                .Where(p => p.Categories.Any(c => c.Slug == categorySlug))
                .OrderByDescending(p => p.PublishedAt)
                .ToList();

            return await Task.FromResult(posts);
        }

        public async Task<List<PostDto>> GetPostsByTagAsync(string tagSlug)
        {
            var posts = _posts
                .Where(p => p.Tags.Any(t => t.Slug == tagSlug))
                .OrderByDescending(p => p.PublishedAt)
                .ToList();

            return await Task.FromResult(posts);
        }

        public async Task<PostDto?> GetPostByIdAsync(Guid id)
        {
            var post = _posts.FirstOrDefault(p => p.Id == id);

            if (post == null)
                return await Task.FromResult<PostDto?>(null);

            // Incr�menter les vues
            post.ViewCount++;

            return await Task.FromResult<PostDto?>(post);
        }

        public async Task<PostDto?> GetPostBySlugAsync(string slug)
        {
            var post = _posts.FirstOrDefault(p => p.Slug == slug);

            if (post == null)
                return await Task.FromResult<PostDto?>(null);

            // Incr�menter les vues
            post.ViewCount++;

            return await Task.FromResult<PostDto?>(post);
        }

        public async Task<PostDto?> UpdatePostAsync(Guid id, UpdatePostRequest request)
        {
            var post = _posts.FirstOrDefault(p => p.Id == id);

            if (post == null)
                return await Task.FromResult<PostDto?>(null);

            post.Title = request.Title;
            post.Slug = GenerateSlug(request.Title);
            post.Content = request.Content;
            post.Excerpt = request.Excerpt;
            post.CoverImageUrl = request.CoverImageUrl;
            post.PostType = request.PostType;

            return await Task.FromResult<PostDto?>(post);
        }

        public async Task<List<PostDto>> SearchPostsAsync(string query)
        {
            var posts = _posts
                .Where(p =>
                    p.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    p.Excerpt.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    p.AuthorName.Contains(query, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(p => p.PublishedAt)
                .ToList();

            return await Task.FromResult(posts);
        }

        private string GenerateSlug(string title)
        {
            return title
                .ToLowerInvariant()
                .Replace(" ", "-")
                .Replace("�", "a")
                .Replace("�", "e")
                .Replace("�", "e")
                .Replace("�", "e")
                .Replace("�", "e")
                .Replace("�", "i")
                .Replace("�", "i")
                .Replace("�", "o")
                .Replace("�", "u")
                .Replace("�", "u")
                .Replace("�", "c")
                .Replace("'", "-")
                .Replace("\"", "")
                .Replace(",", "")
                .Replace(".", "")
                .Replace("?", "")
                .Replace("!", "");
        }
    }
}

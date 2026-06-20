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
                    Title = "Les nouveautï¿½s de .NET 9",
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
                        new TagDto { Id = Guid.NewGuid(), Name = ".NET9", Slug = "dotnet9", UsageCount = 5 },
                        new TagDto { Id = Guid.NewGuid(), Name = "C#", Slug = "csharp", UsageCount = 15 }
                    }
                },
                new PostDto
                {
                    Id = Guid.NewGuid(),
                    Title = "Introduction ï¿½ Blazor WebAssembly",
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
                        new TagDto { Id = Guid.NewGuid(), Name = "Blazor", Slug = "blazor", UsageCount = 8 },
                        new TagDto { Id = Guid.NewGuid(), Name = "WebAssembly", Slug = "webassembly", UsageCount = 6 }
                    }
                }
            };
        }

        public async Task<PostDto?> CreatePostAsync(CreatePostRequest request, Guid CurrentId)
        {
            var newPost = new PostDto
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Slug = GenerateSlug(request.Title),
                Excerpt = request.Excerpt,
                Content = request.Content,
                CoverImageUrl = request.CoverImageUrl ?? "/images/default.jpg",
                AuthorId = CurrentId, // ï¿½ remplacer par l'utilisateur connectï¿½
                AuthorName = "Admin",
                AuthorAvatar = "/images/avatars/default.jpg",
                PostType = request.PostType,
                PublishedAt = DateTime.Now,
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
            await Task.Delay(800);
            var posts = _posts
                .OrderByDescending(p => p.PublishedAt)
                .ToList();

            return await Task.FromResult(posts);
        }
  
        public async Task<List<PostDto>> GetPublishedPostsAsync()
        {
            await Task.Delay(800);
            var posts = _posts
                .Where(p => p.PublishedAt != DateTime.MinValue)
                .OrderByDescending(p => p.PublishedAt)
                .ToList();

            return await Task.FromResult(posts);
        }

        public async Task<List<PostDto>> GetPostsByCategoryAsync(string categorySlug)
        {
            await Task.Delay(800);
            var posts = _posts
                .Where(p => p.Categories.Any(c => c.Slug == categorySlug))
                .OrderByDescending(p => p.PublishedAt)
                .ToList();

            return await Task.FromResult(posts);
        }

        public async Task<List<PostDto>> GetPostsByTagAsync(string tagSlug)
        {
            await Task.Delay(800);
            var posts = _posts
                .Where(p => p.Tags.Any(t => t.Slug == tagSlug))
                .OrderByDescending(p => p.PublishedAt)
                .ToList();

            return await Task.FromResult(posts);
        }

        public async Task<PostDto?> GetPostByIdAsync(Guid id)
        {
            await Task.Delay(800);
            var post = _posts.FirstOrDefault(p => p.Id == id);

            if (post == null)
                return await Task.FromResult<PostDto?>(null);

            return await Task.FromResult<PostDto?>(post);
        }

        public async Task<PostDto?> GetPostBySlugAsync(string slug)
        {
            await Task.Delay(800);
            var post = _posts.FirstOrDefault(p => p.Slug == slug);

            if (post == null)
                return await Task.FromResult<PostDto?>(null);

            return await Task.FromResult<PostDto?>(post);
        }

        public async Task IncrementViewCountAsync(Guid id)
        {
            var post = _posts.FirstOrDefault(p => p.Id == id);
            if (post is not null) post.ViewCount++;
            await Task.CompletedTask;
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
            await Task.Delay(800);
            var posts = _posts
                .Where(p =>
                    p.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    p.Excerpt.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    p.AuthorName.Contains(query, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(p => p.PublishedAt)
                .ToList();

            return await Task.FromResult(posts);
        }

        // nouveaux fonctionnalité

        public async Task<bool> PublishPostAsync(Guid postId)
        {
            await Task.Delay(300);
            var post = _posts.FirstOrDefault(p => p.Id == postId);
            if(post == null) return false;

            post.PublishedAt = DateTime.Now;
            return true;
        }

        public async Task<bool> UnPublishPostAsync (Guid postId)
        {
            await Task.Delay(300);
            var post = _posts.FirstOrDefault(p => p.Id == postId);
            if(post == null) return false;

            post.PublishedAt = null;
            return true;

        }

        private string GenerateSlug(string title)
        {
            return title
                .ToLowerInvariant()
                .Replace(" ", "-")
                .Replace("ï¿½", "a")
                .Replace("ï¿½", "e")
                .Replace("ï¿½", "e")
                .Replace("ï¿½", "e")
                .Replace("ï¿½", "e")
                .Replace("ï¿½", "i")
                .Replace("ï¿½", "i")
                .Replace("ï¿½", "o")
                .Replace("ï¿½", "u")
                .Replace("ï¿½", "u")
                .Replace("ï¿½", "c")
                .Replace("'", "-")
                .Replace("\"", "")
                .Replace(",", "")
                .Replace(".", "")
                .Replace("?", "")
                .Replace("!", "");
        }
    }
}

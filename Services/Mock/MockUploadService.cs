using DotnetNiger.UI.Models;
using DotnetNiger.UI.Models.Responses;
using DotnetNiger.UI.Services.Contracts;
using Microsoft.AspNetCore.Components.Forms;

namespace DotnetNiger.UI.Services.Mock;

public class MockUploadService : IUploadService
{
    private readonly ISessionStorageService _localStorage;
    private const long MaxFileSize = 5 * 1024 * 1024;
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp", ".gif"
    };
    private static readonly HashSet<string> AllowedMimeTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg", "image/png", "image/webp", "image/gif"
    };

    private const string StorageKeyPrefix = "uploaded_images";

    public MockUploadService(ISessionStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task<UploadResponse> UploadImageAsync(IBrowserFile file, UploadType type)
    {
        var extension = Path.GetExtension(file.Name);
        var mimeType = file.ContentType;

        if (!AllowedExtensions.Contains(extension))
        {
            return new UploadResponse
            {
                Success = false,
                Message = $"Format non autorisé. Formats acceptés : {string.Join(", ", AllowedExtensions)}"
            };
        }

        if (!AllowedMimeTypes.Contains(mimeType))
        {
            return new UploadResponse
            {
                Success = false,
                Message = "Type MIME non autorisé."
            };
        }

        if (file.Size > MaxFileSize)
        {
            return new UploadResponse
            {
                Success = false,
                Message = $"Le fichier dépasse la taille maximale de 5 Mo."
            };
        }

        using var memoryStream = new MemoryStream();
        await file.OpenReadStream(MaxFileSize).CopyToAsync(memoryStream);
        var bytes = memoryStream.ToArray();
        var base64 = Convert.ToBase64String(bytes);

        var uniqueName = $"{Guid.NewGuid()}{extension}";
        var folder = GetFolderPath(type);
        var relativePath = $"{folder}/{uniqueName}";

        var imageData = new MockImageData
        {
            FileName = uniqueName,
            Base64Content = base64,
            ContentType = mimeType,
            FileSize = file.Size,
            UploadedAt = DateTime.UtcNow
        };

        await SaveImageAsync(relativePath, imageData);

        return new UploadResponse
        {
            Success = true,
            Message = "Image uploadée avec succès.",
            ImageUrl = relativePath,
            FileName = uniqueName,
            FileSize = file.Size,
            UploadedAt = DateTime.UtcNow
        };
    }

    public async Task<UploadResponse> UploadImageBase64Async(string base64Content, string fileName, UploadType type)
    {
        var extension = Path.GetExtension(fileName);

        if (!AllowedExtensions.Contains(extension))
        {
            return new UploadResponse
            {
                Success = false,
                Message = $"Format non autorisé. Formats acceptés : {string.Join(", ", AllowedExtensions)}"
            };
        }

        var bytes = Convert.FromBase64String(base64Content);

        if (bytes.Length > MaxFileSize)
        {
            return new UploadResponse
            {
                Success = false,
                Message = $"Le fichier dépasse la taille maximale de 5 Mo."
            };
        }

        var uniqueName = $"{Guid.NewGuid()}{extension}";
        var folder = GetFolderPath(type);
        var relativePath = $"{folder}/{uniqueName}";

        var imageData = new MockImageData
        {
            FileName = uniqueName,
            Base64Content = base64Content,
            ContentType = GetMimeType(extension),
            FileSize = bytes.Length,
            UploadedAt = DateTime.UtcNow
        };

        await SaveImageAsync(relativePath, imageData);

        return new UploadResponse
        {
            Success = true,
            Message = "Image uploadée avec succès.",
            ImageUrl = relativePath,
            FileName = uniqueName,
            FileSize = bytes.Length,
            UploadedAt = DateTime.UtcNow
        };
    }

    public async Task<bool> DeleteImageAsync(string imageUrl)
    {
        var key = $"{StorageKeyPrefix}:{imageUrl}";
        await _localStorage.RemoveItemAsync(key);

        var indexKey = $"{StorageKeyPrefix}:index";
        var index = await _localStorage.GetItemAsync<List<string>>(indexKey);
        if (index is not null && index.Remove(imageUrl))
        {
            await _localStorage.SetItemAsync(indexKey, index);
        }

        return true;
    }

    public async Task<string?> ResolveImageUrlAsync(string imageUrl)
    {
        var key = $"{StorageKeyPrefix}:{imageUrl}";
        var data = await _localStorage.GetItemAsync<MockImageData>(key);
        if (data is null)
            return null;

        return $"data:{data.ContentType};base64,{data.Base64Content}";
    }

    public string GetFolderPath(UploadType type) => type switch
    {
        UploadType.User => "/uploads/users",
        UploadType.Event => "/uploads/events",
        UploadType.Blog => "/uploads/blog",
        _ => "/uploads"
    };

    private async Task SaveImageAsync(string relativePath, MockImageData data)
    {
        var key = $"{StorageKeyPrefix}:{relativePath}";
        await _localStorage.SetItemAsync(key, data);

        var indexKey = $"{StorageKeyPrefix}:index";
        var index = await _localStorage.GetItemAsync<List<string>>(indexKey) ?? new List<string>();
        if (!index.Contains(relativePath))
        {
            index.Add(relativePath);
            await _localStorage.SetItemAsync(indexKey, index);
        }
    }

    private static string GetMimeType(string extension) => extension.ToLowerInvariant() switch
    {
        ".jpg" or ".jpeg" => "image/jpeg",
        ".png" => "image/png",
        ".webp" => "image/webp",
        ".gif" => "image/gif",
        _ => "application/octet-stream"
    };

    private class MockImageData
    {
        public string FileName { get; set; } = string.Empty;
        public string Base64Content { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}

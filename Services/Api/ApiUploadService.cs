using System.Net.Http.Json;
using System.Text.Json;
using DotnetNiger.UI.Models;
using DotnetNiger.UI.Models.Responses;
using DotnetNiger.UI.Services.Contracts;
using Microsoft.AspNetCore.Components.Forms;

namespace DotnetNiger.UI.Services.Api;

public class ApiUploadService : ApiServiceBase, IUploadService
{
    private const long MaxFileSize = 5 * 1024 * 1024;
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp", ".gif"
    };

    public ApiUploadService(HttpClient http) : base(http)
    {
    }

    public async Task<UploadResponse> UploadImageAsync(IBrowserFile file, UploadType type)
    {
        var extension = Path.GetExtension(file.Name);

        if (!AllowedExtensions.Contains(extension))
        {
            return new UploadResponse
            {
                Success = false,
                Message = $"Format non autorisé. Formats acceptés : {string.Join(", ", AllowedExtensions)}"
            };
        }

        if (file.Size > MaxFileSize)
        {
            return new UploadResponse
            {
                Success = false,
                Message = "Le fichier dépasse la taille maximale de 5 Mo."
            };
        }

        using var content = new MultipartFormDataContent();
        using var stream = file.OpenReadStream(MaxFileSize);
        var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
        content.Add(fileContent, "file", file.Name);

        var response = await Http.PostAsync($"{ApiEndpoints.Upload}?type={type}", content);

        if (!response.IsSuccessStatusCode)
        {
            return new UploadResponse
            {
                Success = false,
                Message = $"Erreur lors de l'upload : {response.StatusCode}"
            };
        }

        var result = await response.Content.ReadFromJsonAsync<UploadResponse>(new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return result ?? new UploadResponse
        {
            Success = false,
            Message = "Réponse inattendue du serveur."
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

        var request = new
        {
            fileName,
            base64Content,
            type = type.ToString()
        };

        var response = await Http.PostAsJsonAsync(ApiEndpoints.UploadBase64, request);

        if (!response.IsSuccessStatusCode)
        {
            return new UploadResponse
            {
                Success = false,
                Message = $"Erreur lors de l'upload : {response.StatusCode}"
            };
        }

        var result = await response.Content.ReadFromJsonAsync<UploadResponse>(new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return result ?? new UploadResponse
        {
            Success = false,
            Message = "Réponse inattendue du serveur."
        };
    }

    public async Task<bool> DeleteImageAsync(string imageUrl)
    {
        var response = await Http.DeleteAsync($"{ApiEndpoints.Upload}?path={Uri.EscapeDataString(imageUrl)}");
        return response.IsSuccessStatusCode;
    }

    public Task<string?> ResolveImageUrlAsync(string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            return Task.FromResult<string?>(null);

        if (Uri.TryCreate(imageUrl, UriKind.Absolute, out _))
            return Task.FromResult<string?>(imageUrl);

        var baseUri = Http.BaseAddress?.ToString().TrimEnd('/');
        return Task.FromResult<string?>($"{baseUri}{imageUrl}");
    }

    public string GetFolderPath(UploadType type) => type switch
    {
        UploadType.User => "/uploads/users",
        UploadType.Event => "/uploads/events",
        UploadType.Blog => "/uploads/blog",
        _ => "/uploads"
    };
}

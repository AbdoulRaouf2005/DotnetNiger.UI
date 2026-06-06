using DotnetNiger.UI.Models;
using DotnetNiger.UI.Models.Responses;
using Microsoft.AspNetCore.Components.Forms;

namespace DotnetNiger.UI.Services.Contracts;

public interface IUploadService
{
    Task<UploadResponse> UploadImageAsync(IBrowserFile file, UploadType type);
    Task<UploadResponse> UploadImageBase64Async(string base64Content, string fileName, UploadType type);
    Task<bool> DeleteImageAsync(string imageUrl);
    Task<string?> ResolveImageUrlAsync(string imageUrl);
    string GetFolderPath(UploadType type);
}

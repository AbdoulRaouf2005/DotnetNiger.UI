using DotnetNiger.UI.Models;

namespace DotnetNiger.UI.Models.Requests;

public class UploadRequest
{
    public string FileName { get; set; } = string.Empty;
    public string Base64Content { get; set; } = string.Empty;
    public UploadType Type { get; set; }
}

using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace DotnetNiger.UI.Services.Auth;

public static class JwtParser
{
    public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var parts = jwt.Split('.');
        if (parts.Length < 3)
            return Enumerable.Empty<Claim>();

        var payload = parts[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var kvs = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonBytes)!;

        return kvs.SelectMany(kv =>
        {
            if (kv.Key is "roles" or "role")
            {
                if (kv.Value.ValueKind == JsonValueKind.Array)
                    return kv.Value.EnumerateArray().Select(r => new Claim(ClaimTypes.Role, r.GetString()!));
                return new[] { new Claim(ClaimTypes.Role, kv.Value.GetString()!) };
            }

            return new[] { new Claim(kv.Key, kv.Value.ToString()) };
        });
    }

    public static byte[] ParseBase64WithoutPadding(string base64)
    {
        base64 = base64.Replace('-', '+').Replace('_', '/');
        return (base64.Length % 4) switch
        {
            2 => Convert.FromBase64String(base64 + "=="),
            3 => Convert.FromBase64String(base64 + "="),
            _ => Convert.FromBase64String(base64),
        };
    }
}

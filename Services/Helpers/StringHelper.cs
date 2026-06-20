namespace DotnetNiger.UI.Services.Helpers;

public static class StringHelper
{
    public static string? FirstNotEmpty(params string?[] values)
        => values.FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));

    public static string? NormalizeImagePath(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        if (value.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            || value.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
            || value.StartsWith("/"))
        {
            return value;
        }

        return $"/{value.TrimStart('/')}";
    }

    public static string GetInitials(string displayName)
    {
        var words = displayName
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Take(2)
            .ToArray();

        if (words.Length == 0) return "U";

        return string.Concat(words.Select(w => char.ToUpperInvariant(w[0])));
    }

    public static string GenerateSlug(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return string.Empty;

        var normalized = title
            .Replace("é", "e").Replace("è", "e").Replace("ê", "e").Replace("ë", "e")
            .Replace("à", "a").Replace("â", "a").Replace("ä", "a")
            .Replace("ô", "o").Replace("ö", "o")
            .Replace("ù", "u").Replace("û", "u").Replace("ü", "u")
            .Replace("î", "i").Replace("ï", "i")
            .Replace("ç", "c")
            .ToLowerInvariant();

        var slug = new System.Text.StringBuilder();
        foreach (char c in normalized)
        {
            if (char.IsLetterOrDigit(c))
                slug.Append(c);
            else if (c == ' ' || c == '-')
                slug.Append('-');
        }

        var result = slug.ToString();
        while (result.Contains("--"))
            result = result.Replace("--", "-");

        return result.Trim('-');
    }
}

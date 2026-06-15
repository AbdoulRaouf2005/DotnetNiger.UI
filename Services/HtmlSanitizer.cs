using System.Text.RegularExpressions;

namespace DotnetNiger.UI.Services;

public static partial class HtmlSanitizer
{
    private static readonly HashSet<string> AllowedTags =
    [
        "p", "br", "b", "i", "u", "em", "strong", "small", "sub", "sup",
        "h1", "h2", "h3", "h4", "h5", "h6",
        "ul", "ol", "li", "dl", "dt", "dd",
        "a", "img",
        "blockquote", "pre", "code", "hr",
        "table", "thead", "tbody", "tfoot", "tr", "th", "td",
        "div", "span", "section", "article", "header", "footer",
        "figure", "figcaption", "mark", "time", "abbr", "cite"
    ];

    private static readonly HashSet<string> AllowedAttributes =
    [
        "href", "src", "alt", "title", "rel", "target",
        "class", "id", "name", "width", "height",
        "download", "hreflang", "type", "charset",
        "datetime", "cite", "colspan", "rowspan"
    ];

    private static readonly Regex StripTagsRx = StripTagsRegex();
    private static readonly Regex TagRx = TagRegex();
    private static readonly Regex AttrRx = AttrRegex();
    private static readonly Regex AttrNameRx = AttrNameRegex();
    private static readonly Regex DangerousAttrRx = DangerousAttrRegex();
    private static readonly Regex CssUrlRx = CssUrlRegex();

    public static string Sanitize(string html)
    {
        if (string.IsNullOrWhiteSpace(html))
            return string.Empty;

        html = StripComments(html);
        html = CssUrlRx.Replace(html, "url()");
        html = DangerousAttrRx.Replace(html, "");

        return TagRx.Replace(html, EvaluateTag);
    }

    private static string EvaluateTag(Match match)
    {
        var fullTag = match.Value;

        var tagParts = TagPartsRegex().Match(fullTag);
        if (!tagParts.Success)
            return "";

        var tagName = tagParts.Groups[1].Value.ToLowerInvariant();
        if (!AllowedTags.Contains(tagName))
            return "";

        if (fullTag.StartsWith("</"))
            return fullTag;

        var attrs = tagParts.Groups[2].Value;
        var cleanAttrs = SanitizeAttributes(attrs);

        if (fullTag.EndsWith("/>"))
            return $"<{tagName} {cleanAttrs}/>".TrimEnd(' ').Replace(" />", " />");
        else
            return $"<{tagName}{cleanAttrs}>";
    }

    private static string SanitizeAttributes(string attrs)
    {
        if (string.IsNullOrWhiteSpace(attrs))
            return "";

        var keep = new List<string>();
        foreach (Match match in AttrRx.Matches(attrs))
        {
            var name = AttrNameRx.Match(match.Value).Value.ToLowerInvariant();
            var value = match.Groups[2].Value;

            if (name.StartsWith("on"))
                continue;

            if (name == "style")
                continue;

            if (AllowedAttributes.Contains(name) && !HasJavascriptProtocol(value))
                keep.Add(match.Value);
        }

        return keep.Count > 0 ? " " + string.Join(" ", keep) : "";
    }

    private static bool HasJavascriptProtocol(string value)
    {
        var trimmed = value.Trim().ToLowerInvariant();
        return trimmed.StartsWith("javascript:") || trimmed.StartsWith("vbscript:") || trimmed.StartsWith("data:");
    }

    private static string StripComments(string html)
    {
        int idx;
        while ((idx = html.IndexOf("<!--", StringComparison.Ordinal)) >= 0)
        {
            var endIdx = html.IndexOf("-->", idx, StringComparison.Ordinal);
            if (endIdx < 0)
                break;
            html = html.Remove(idx, endIdx + 3 - idx);
        }
        return html;
    }

    [GeneratedRegex(@"<[^>]*?>", RegexOptions.Compiled)]
    private static partial Regex StripTagsRegex();

    [GeneratedRegex(@"<\/?(\w+)[^>]*?>", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex TagRegex();

    [GeneratedRegex(@"\s+(\w[-.\w]*)\s*=\s*""([^""]*)""", RegexOptions.Compiled)]
    private static partial Regex AttrRegex();

    [GeneratedRegex(@"^(\w[-.\w]*)", RegexOptions.Compiled)]
    private static partial Regex AttrNameRegex();

    [GeneratedRegex(@"(on\w+\s*=\s*""[^""]*""|on\w+\s*=\s*'[^']*'|javascript:\s*[^""\s>]*)", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex DangerousAttrRegex();

    [GeneratedRegex(@"url\s*\(\s*(?:""[^""]*""|'[^']*'|[^)]*)\s*\)", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex CssUrlRegex();

    [GeneratedRegex(@"^<(\w+)([\s\S]*)$", RegexOptions.Compiled)]
    private static partial Regex TagPartsRegex();
}

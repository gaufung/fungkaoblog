using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;

namespace Blog.Api.Services;

public sealed class GitHubBlogClient
{
    private static readonly Regex FileNamePattern = new(
        @"^(?<number>\d+)-.+\.md$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

    private readonly HttpClient _httpClient;
    private readonly GitHubBlogSyncOptions _options;

    public GitHubBlogClient(HttpClient httpClient, IOptions<GitHubBlogSyncOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<IReadOnlyList<GitHubBlogDocument>> GetDocumentsAsync(
        CancellationToken cancellationToken)
    {
        var items = await GetAsync<List<GitHubContentItem>>(
            BuildContentsUrl(_options.Folder),
            cancellationToken);

        var markdownFiles = items
            .Where(item =>
                string.Equals(item.Type, "file", StringComparison.OrdinalIgnoreCase) &&
                item.Name.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
            .ToList();

        var documents = await Task.WhenAll(markdownFiles.Select(item =>
            DownloadDocumentAsync(item, cancellationToken)));

        return documents;
    }

    private async Task<GitHubBlogDocument> DownloadDocumentAsync(
        GitHubContentItem item,
        CancellationToken cancellationToken)
    {
        var fileMatch = FileNamePattern.Match(item.Name);
        if (!fileMatch.Success ||
            !int.TryParse(
                fileMatch.Groups["number"].Value,
                NumberStyles.None,
                CultureInfo.InvariantCulture,
                out var sourceNumber))
        {
            throw new InvalidOperationException(
                $"GitHub blog filename '{item.Name}' must start with a numeric prefix followed by '-'.");
        }

        var content = await GetAsync<GitHubFileContent>(
            BuildContentsUrl(item.Path),
            cancellationToken);

        if (!string.Equals(content.Encoding, "base64", StringComparison.OrdinalIgnoreCase) ||
            string.IsNullOrWhiteSpace(content.Content))
        {
            throw new InvalidOperationException(
                $"GitHub did not return base64 content for '{item.Path}'.");
        }

        var bytes = Convert.FromBase64String(
            content.Content.Replace("\n", string.Empty, StringComparison.Ordinal));
        var markdown = Encoding.UTF8.GetString(bytes);

        return BlogFrontMatterParser.Parse(item.Name, sourceNumber, markdown);
    }

    private async Task<T> GetAsync<T>(string url, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
        request.Headers.Add("X-GitHub-Api-Version", "2022-11-28");

        if (!string.IsNullOrWhiteSpace(_options.Token))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", _options.Token);
        }

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<T>(cancellationToken)
               ?? throw new InvalidOperationException(
                   $"GitHub returned an empty response for '{url}'.");
    }

    private string BuildContentsUrl(string path)
    {
        var escapedPath = string.Join(
            '/',
            path.Split('/', StringSplitOptions.RemoveEmptyEntries)
                .Select(Uri.EscapeDataString));

        return $"https://api.github.com/repos/{Uri.EscapeDataString(_options.Owner)}/" +
               $"{Uri.EscapeDataString(_options.Repository)}/contents/{escapedPath}";
    }

    private sealed record GitHubContentItem(
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("path")] string Path,
        [property: JsonPropertyName("type")] string Type);

    private sealed record GitHubFileContent(
        [property: JsonPropertyName("content")] string? Content,
        [property: JsonPropertyName("encoding")] string? Encoding);
}

public sealed record GitHubBlogDocument(
    int SourceNumber,
    string Title,
    IReadOnlyList<string> Tags,
    bool Published,
    string Content);

internal static class BlogFrontMatterParser
{
    public static GitHubBlogDocument Parse(
        string fileName,
        int sourceNumber,
        string markdown)
    {
        var normalized = markdown
            .TrimStart('\uFEFF')
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace('\r', '\n');
        var lines = normalized.Split('\n');

        if (lines.Length == 0 || !string.Equals(lines[0].Trim(), "---", StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"GitHub blog '{fileName}' does not start with YAML front matter.");
        }

        var closingDelimiter = Array.FindIndex(
            lines,
            1,
            line => string.Equals(line.Trim(), "---", StringComparison.Ordinal));
        if (closingDelimiter < 0)
        {
            throw new InvalidOperationException(
                $"GitHub blog '{fileName}' has unterminated YAML front matter.");
        }

        var metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        for (var index = 1; index < closingDelimiter; index++)
        {
            var line = lines[index];
            var separator = line.IndexOf(':');
            if (separator <= 0)
            {
                continue;
            }

            metadata[line[..separator].Trim()] = Unquote(line[(separator + 1)..].Trim());
        }

        if (!metadata.TryGetValue("name", out var title) || string.IsNullOrWhiteSpace(title))
        {
            throw new InvalidOperationException(
                $"GitHub blog '{fileName}' is missing the 'name' metadata.");
        }

        if (!metadata.TryGetValue("status", out var status) || string.IsNullOrWhiteSpace(status))
        {
            throw new InvalidOperationException(
                $"GitHub blog '{fileName}' is missing the 'status' metadata.");
        }

        metadata.TryGetValue("tags", out var tagsValue);
        if (string.IsNullOrWhiteSpace(tagsValue))
        {
            // Existing source files use this typo; keep compatibility while accepting "tags".
            metadata.TryGetValue("tages", out tagsValue);
        }

        var tags = ParseTags(tagsValue);
        var content = string.Join('\n', lines.Skip(closingDelimiter + 1)).TrimStart('\n');

        return new GitHubBlogDocument(
            sourceNumber,
            title.Trim(),
            tags,
            string.Equals(status.Trim(), "publish", StringComparison.OrdinalIgnoreCase),
            content);
    }

    private static IReadOnlyList<string> ParseTags(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return [];
        }

        var normalized = value.Trim();
        if (normalized.StartsWith('[') && normalized.EndsWith(']'))
        {
            normalized = normalized[1..^1];
        }

        return normalized
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(Unquote)
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static string Unquote(string value)
    {
        if (value.Length >= 2 &&
            ((value[0] == '"' && value[^1] == '"') ||
             (value[0] == '\'' && value[^1] == '\'')))
        {
            return value[1..^1];
        }

        return value;
    }
}

namespace Blog.Api.Services;

public sealed class GitHubBlogSyncOptions
{
    public const string SectionName = "GitHubBlogSync";

    public string Owner { get; set; } = string.Empty;

    public string Repository { get; set; } = string.Empty;

    public string Folder { get; set; } = string.Empty;

    public TimeSpan Interval { get; set; } = TimeSpan.FromHours(6);

    public string? Token { get; set; }
}

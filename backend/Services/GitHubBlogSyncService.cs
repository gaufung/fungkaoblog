using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Blog.Api.Data;
using Blog.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Blog.Api.Services;

public sealed class GitHubBlogSyncService : BackgroundService
{
    private readonly GitHubBlogClient _client;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMemoryCache _cache;
    private readonly GitHubBlogSyncOptions _options;
    private readonly ILogger<GitHubBlogSyncService> _logger;

    public GitHubBlogSyncService(
        GitHubBlogClient client,
        IServiceScopeFactory scopeFactory,
        IMemoryCache cache,
        IOptions<GitHubBlogSyncOptions> options,
        ILogger<GitHubBlogSyncService> logger)
    {
        _client = client;
        _scopeFactory = scopeFactory;
        _cache = cache;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await SyncSafelyAsync(stoppingToken);

        using var timer = new PeriodicTimer(_options.Interval);
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await SyncSafelyAsync(stoppingToken);
        }
    }

    private async Task SyncSafelyAsync(CancellationToken cancellationToken)
    {
        try
        {
            await SyncAsync(cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Failed to sync blog content from {Owner}/{Repository}/{Folder}.",
                _options.Owner,
                _options.Repository,
                _options.Folder);
        }
    }

    private async Task SyncAsync(CancellationToken cancellationToken)
    {
        var documents = await _client.GetDocumentsAsync(cancellationToken);
        var publishedDocuments = ValidateAndGetPublishedDocuments(documents);

        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BlogDbContext>();
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

        var existingPosts = await db.Posts
            .Include(post => post.Tags)
            .ToListAsync(cancellationToken);
        EnsureUniqueDatabaseTitles(existingPosts);

        var documentsByTitle = publishedDocuments.ToDictionary(
            document => document.Title,
            StringComparer.OrdinalIgnoreCase);
        var affectedSlugs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var postsToDelete = existingPosts
            .Where(post => !documentsByTitle.ContainsKey(post.Title))
            .ToList();
        foreach (var post in postsToDelete)
        {
            affectedSlugs.Add(post.Slug);
        }

        db.Posts.RemoveRange(postsToDelete);
        await db.SaveChangesAsync(cancellationToken);

        existingPosts = existingPosts.Except(postsToDelete).ToList();

        // Release unique source-number and slug values before applying the
        // snapshot so renumbered posts can safely exchange identifiers.
        foreach (var post in existingPosts)
        {
            var document = documentsByTitle[post.Title];
            var desiredSlug = CreatePostSlug(document.SourceNumber);
            if (post.SourceNumber == document.SourceNumber &&
                string.Equals(post.Slug, desiredSlug, StringComparison.Ordinal))
            {
                continue;
            }

            affectedSlugs.Add(post.Slug);
            post.SourceNumber = null;
            post.Slug = $"sync-temp-{Guid.NewGuid():N}";
        }

        await db.SaveChangesAsync(cancellationToken);

        var postsByTitle = existingPosts.ToDictionary(
            post => post.Title,
            StringComparer.OrdinalIgnoreCase);

        var tags = await db.Tags.ToListAsync(cancellationToken);
        var tagsByName = tags
            .GroupBy(tag => tag.Name, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);
        var usedTagSlugs = tags
            .Select(tag => tag.Slug)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var now = DateTime.UtcNow;
        foreach (var document in publishedDocuments.OrderBy(document => document.SourceNumber))
        {
            var desiredTags = document.Tags
                .Select(tagName => GetOrCreateTag(
                    tagName,
                    tagsByName,
                    usedTagSlugs,
                    db))
                .ToList();

            if (!postsByTitle.TryGetValue(document.Title, out var post))
            {
                post = new Post
                {
                    Title = document.Title,
                    Slug = CreatePostSlug(document.SourceNumber),
                    Content = document.Content,
                    Published = true,
                    SourceNumber = document.SourceNumber,
                    CreatedAt = now,
                    UpdatedAt = now,
                    Tags = desiredTags
                };
                db.Posts.Add(post);
                postsByTitle.Add(post.Title, post);
                continue;
            }

            var oldSlug = post.Slug;
            var newSlug = CreatePostSlug(document.SourceNumber);
            var desiredTagNames = desiredTags
                .Select(tag => tag.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            var tagsChanged = !post.Tags
                .Select(tag => tag.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase)
                .SetEquals(desiredTagNames);
            var postChanged =
                !string.Equals(post.Title, document.Title, StringComparison.Ordinal) ||
                !string.Equals(post.Slug, newSlug, StringComparison.Ordinal) ||
                !string.Equals(post.Content, document.Content, StringComparison.Ordinal) ||
                !post.Published ||
                post.SourceNumber != document.SourceNumber ||
                tagsChanged;

            if (!postChanged)
            {
                continue;
            }

            post.Title = document.Title;
            post.Slug = newSlug;
            post.Content = document.Content;
            post.Published = true;
            post.SourceNumber = document.SourceNumber;
            post.UpdatedAt = now;

            if (tagsChanged)
            {
                post.Tags.Clear();
                foreach (var tag in desiredTags)
                {
                    post.Tags.Add(tag);
                }
            }

            affectedSlugs.Add(oldSlug);
            affectedSlugs.Add(newSlug);
        }

        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        foreach (var slug in affectedSlugs)
        {
            _cache.Remove($"post:{slug}");
        }

        _logger.LogInformation(
            "Synced {PublishedCount} published blog posts from GitHub; deleted {DeletedCount}.",
            publishedDocuments.Count,
            postsToDelete.Count);
    }

    private static IReadOnlyList<GitHubBlogDocument> ValidateAndGetPublishedDocuments(
        IReadOnlyList<GitHubBlogDocument> documents)
    {
        var published = documents.Where(document => document.Published).ToList();

        var duplicateTitle = published
            .GroupBy(document => document.Title, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault(group => group.Count() > 1);
        if (duplicateTitle is not null)
        {
            throw new InvalidOperationException(
                $"GitHub contains more than one published blog named '{duplicateTitle.Key}'.");
        }

        var duplicateNumber = published
            .GroupBy(document => document.SourceNumber)
            .FirstOrDefault(group => group.Count() > 1);
        if (duplicateNumber is not null)
        {
            throw new InvalidOperationException(
                $"GitHub contains more than one published blog with number {duplicateNumber.Key}.");
        }

        foreach (var document in published)
        {
            if (document.Title.Length > 200)
            {
                throw new InvalidOperationException(
                    $"GitHub blog title '{document.Title}' exceeds 200 characters.");
            }

            var oversizedTag = document.Tags.FirstOrDefault(tag => tag.Length > 50);
            if (oversizedTag is not null)
            {
                throw new InvalidOperationException(
                    $"GitHub tag '{oversizedTag}' exceeds 50 characters.");
            }
        }

        return published;
    }

    private static void EnsureUniqueDatabaseTitles(IReadOnlyList<Post> posts)
    {
        var duplicateTitle = posts
            .GroupBy(post => post.Title, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault(group => group.Count() > 1);
        if (duplicateTitle is not null)
        {
            throw new InvalidOperationException(
                $"The database contains more than one post named '{duplicateTitle.Key}'.");
        }
    }

    private static Tag GetOrCreateTag(
        string name,
        IDictionary<string, Tag> tagsByName,
        ISet<string> usedTagSlugs,
        BlogDbContext db)
    {
        if (tagsByName.TryGetValue(name, out var existingTag))
        {
            return existingTag;
        }

        var baseSlug = Slugify(name);
        var slug = baseSlug;
        var suffix = 2;
        while (!usedTagSlugs.Add(slug))
        {
            var suffixText = $"-{suffix++}";
            slug = $"{baseSlug[..Math.Min(baseSlug.Length, 60 - suffixText.Length)]}{suffixText}";
        }

        var tag = new Tag
        {
            Name = name,
            Slug = slug
        };
        db.Tags.Add(tag);
        tagsByName.Add(name, tag);
        return tag;
    }

    private static string CreatePostSlug(int sourceNumber) =>
        $"post-{sourceNumber.ToString(CultureInfo.InvariantCulture)}";

    private static string Slugify(string value)
    {
        var builder = new StringBuilder();
        var lastWasSeparator = false;

        foreach (var character in value.Normalize(NormalizationForm.FormKC))
        {
            if (char.IsLetterOrDigit(character))
            {
                builder.Append(char.ToLowerInvariant(character));
                lastWasSeparator = false;
            }
            else if (builder.Length > 0 && !lastWasSeparator)
            {
                builder.Append('-');
                lastWasSeparator = true;
            }
        }

        var slug = builder.ToString().Trim('-');
        if (string.IsNullOrEmpty(slug))
        {
            var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)))
                .ToLowerInvariant();
            slug = $"tag-{hash[..12]}";
        }

        return slug[..Math.Min(slug.Length, 60)].TrimEnd('-');
    }
}

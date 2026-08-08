using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Blog.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Api.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[Route("rss.xml")]
public sealed partial class RssController : ControllerBase
{
    private const int FeedItemLimit = 50;
    private const int DescriptionLength = 320;

    private readonly BlogDbContext _db;

    public RssController(BlogDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    [Produces("application/rss+xml")]
    public async Task<ContentResult> Get(CancellationToken cancellationToken)
    {
        var posts = await _db.Posts
            .AsNoTracking()
            .Where(post => post.Published)
            .OrderByDescending(post => post.SourceNumber ?? -1)
            .ThenByDescending(post => post.CreatedAt)
            .Take(FeedItemLimit)
            .Select(post => new
            {
                post.Title,
                post.Slug,
                post.Content,
                post.CreatedAt,
                post.UpdatedAt,
                Tags = post.Tags.OrderBy(tag => tag.Name).Select(tag => tag.Name).ToList()
            })
            .ToListAsync(cancellationToken);

        var siteUrl = new Uri($"{Request.Scheme}://{Request.Host}{Request.PathBase}/");
        var feedUrl = new Uri(siteUrl, "rss.xml");
        XNamespace atom = "http://www.w3.org/2005/Atom";

        var channel = new XElement(
            "channel",
            new XElement("title", "Fung Kao's Blog"),
            new XElement("link", siteUrl),
            new XElement("description", "Latest posts from Fung Kao's Blog"),
            new XElement("language", "zh-cn"),
            new XElement("generator", "Fung Kao's Blog"),
            new XElement(
                atom + "link",
                new XAttribute("href", feedUrl),
                new XAttribute("rel", "self"),
                new XAttribute("type", "application/rss+xml")));

        if (posts.Count > 0)
        {
            channel.Add(new XElement(
                "lastBuildDate",
                FormatRssDate(posts.Max(post => post.UpdatedAt))));
        }

        foreach (var post in posts)
        {
            var postUrl = new Uri(siteUrl, $"#/post/{Uri.EscapeDataString(post.Slug)}");
            var item = new XElement(
                "item",
                new XElement("title", post.Title),
                new XElement("link", postUrl),
                new XElement(
                    "guid",
                    new XAttribute("isPermaLink", "true"),
                    postUrl),
                new XElement("pubDate", FormatRssDate(post.CreatedAt)),
                new XElement("description", CreateDescription(post.Content)));

            foreach (var tag in post.Tags)
            {
                item.Add(new XElement("category", tag));
            }

            channel.Add(item);
        }

        var document = new XDocument(
            new XDeclaration("1.0", "utf-8", null),
            new XElement(
                "rss",
                new XAttribute("version", "2.0"),
                new XAttribute(XNamespace.Xmlns + "atom", atom),
                channel));

        return Content(document.ToString(), "application/rss+xml; charset=utf-8");
    }

    private static string FormatRssDate(DateTime value) =>
        value.ToUniversalTime().ToString("R", CultureInfo.InvariantCulture);

    private static string CreateDescription(string markdown)
    {
        var description = FencedCodeBlockRegex().Replace(markdown, " ");
        description = ImageRegex().Replace(description, "$1");
        description = LinkRegex().Replace(description, "$1");
        description = MarkdownSyntaxRegex().Replace(description, " ");
        description = WhitespaceRegex().Replace(description, " ").Trim();

        return description.Length <= DescriptionLength
            ? description
            : $"{description[..(DescriptionLength - 1)].TrimEnd()}…";
    }

    [GeneratedRegex(@"(?ms)^[ \t]*(`{3,}|~{3,}).*?^[ \t]*\1[ \t]*$")]
    private static partial Regex FencedCodeBlockRegex();

    [GeneratedRegex(@"!\[([^\]]*)\]\([^)]+\)")]
    private static partial Regex ImageRegex();

    [GeneratedRegex(@"\[([^\]]+)\]\([^)]+\)")]
    private static partial Regex LinkRegex();

    [GeneratedRegex(@"(?m)^[ \t]*[#>*+\-]+[ \t]*|[`*_~]")]
    private static partial Regex MarkdownSyntaxRegex();

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceRegex();
}

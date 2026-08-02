using System.Text;
using System.Text.RegularExpressions;
using Blog.Api.Data;
using Blog.Api.Dtos;
using Blog.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly BlogDbContext _db;

    public PostsController(BlogDbContext db)
    {
        _db = db;
    }

    // Public: list published posts (newest first).
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<PostSummaryDto>>> GetAll([FromQuery] bool includeDrafts = false)
    {
        var query = _db.Posts.AsQueryable();

        // Only a super admin may request drafts.
        if (!includeDrafts || !User.IsInRole("SuperAdmin"))
        {
            query = query.Where(p => p.Published);
        }

        var posts = await query
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PostSummaryDto(p.Id, p.Title, p.Slug, p.CreatedAt, p.UpdatedAt, p.Published))
            .ToListAsync();

        return Ok(posts);
    }

    // Public: read a single post by slug.
    [HttpGet("{slug}")]
    [AllowAnonymous]
    public async Task<ActionResult<PostDto>> GetBySlug(string slug)
    {
        var post = await _db.Posts.FirstOrDefaultAsync(p => p.Slug == slug);
        if (post is null)
        {
            return NotFound();
        }

        if (!post.Published && !User.IsInRole("SuperAdmin"))
        {
            return NotFound();
        }

        return Ok(ToDto(post));
    }

    // SuperAdmin only: create a post.
    [HttpPost]
    [Authorize(Policy = "SuperAdmin")]
    public async Task<ActionResult<PostDto>> Create(PostInputDto input)
    {
        var slug = await EnsureUniqueSlugAsync(string.IsNullOrWhiteSpace(input.Slug) ? Slugify(input.Title) : Slugify(input.Slug!));

        var post = new Post
        {
            Title = input.Title,
            Slug = slug,
            Content = input.Content,
            Published = input.Published,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.Posts.Add(post);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBySlug), new { slug = post.Slug }, ToDto(post));
    }

    // SuperAdmin only: update a post.
    [HttpPut("{id:int}")]
    [Authorize(Policy = "SuperAdmin")]
    public async Task<ActionResult<PostDto>> Update(int id, PostInputDto input)
    {
        var post = await _db.Posts.FindAsync(id);
        if (post is null)
        {
            return NotFound();
        }

        var desiredSlug = string.IsNullOrWhiteSpace(input.Slug) ? Slugify(input.Title) : Slugify(input.Slug!);
        if (desiredSlug != post.Slug)
        {
            post.Slug = await EnsureUniqueSlugAsync(desiredSlug, post.Id);
        }

        post.Title = input.Title;
        post.Content = input.Content;
        post.Published = input.Published;
        post.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return Ok(ToDto(post));
    }

    // SuperAdmin only: delete a post.
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "SuperAdmin")]
    public async Task<IActionResult> Delete(int id)
    {
        var post = await _db.Posts.FindAsync(id);
        if (post is null)
        {
            return NotFound();
        }

        _db.Posts.Remove(post);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static PostDto ToDto(Post p) =>
        new(p.Id, p.Title, p.Slug, p.Content, p.CreatedAt, p.UpdatedAt, p.Published);

    private async Task<string> EnsureUniqueSlugAsync(string baseSlug, int? ignoreId = null)
    {
        var slug = string.IsNullOrWhiteSpace(baseSlug) ? "post" : baseSlug;
        var candidate = slug;
        var suffix = 2;
        while (await _db.Posts.AnyAsync(p => p.Slug == candidate && p.Id != ignoreId))
        {
            candidate = $"{slug}-{suffix++}";
        }
        return candidate;
    }

    private static string Slugify(string value)
    {
        value = value.Trim().ToLowerInvariant();
        value = Regex.Replace(value, @"[^a-z0-9\s-]", string.Empty);
        value = Regex.Replace(value, @"[\s-]+", "-").Trim('-');
        return value;
    }
}

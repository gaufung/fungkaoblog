using Blog.Api.Data;
using Blog.Api.Dtos;
using Blog.Api.Models;
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

    // Public: list published posts (newest first), paginated. Optionally
    // filtered to a single tag by its slug.
    [HttpGet]
    public async Task<ActionResult<PagedResult<PostSummaryDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 8,
        [FromQuery] string? tag = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 8;

        var published = _db.Posts.Where(p => p.Published);

        if (!string.IsNullOrWhiteSpace(tag))
        {
            published = published.Where(p => p.Tags.Any(t => t.Slug == tag));
        }

        var total = await published.CountAsync();

        var items = await published
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new PostSummaryDto(
                p.Id, p.Title, p.Slug, p.CreatedAt, p.UpdatedAt, p.Published,
                p.Tags.OrderBy(t => t.Name).Select(t => new TagDto(t.Name, t.Slug)).ToList()))
            .ToListAsync();

        return Ok(new PagedResult<PostSummaryDto>(items, total, page, pageSize));
    }

    // Public: read a single published post by slug.
    [HttpGet("{slug}")]
    public async Task<ActionResult<PostDto>> GetBySlug(string slug)
    {
        var post = await _db.Posts
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Slug == slug && p.Published);
        if (post is null)
        {
            return NotFound();
        }

        return Ok(ToDto(post));
    }

    private static PostDto ToDto(Post p) =>
        new(p.Id, p.Title, p.Slug, p.Content, p.CreatedAt, p.UpdatedAt, p.Published,
            p.Tags.OrderBy(t => t.Name).Select(t => new TagDto(t.Name, t.Slug)).ToList());
}

namespace Blog.Api.Dtos;

public record PostSummaryDto(int Id, string Title, string Slug, DateTime CreatedAt, DateTime UpdatedAt, bool Published);

public record PostDto(int Id, string Title, string Slug, string Content, DateTime CreatedAt, DateTime UpdatedAt, bool Published);

public record PostInputDto(string Title, string? Slug, string Content, bool Published = true);

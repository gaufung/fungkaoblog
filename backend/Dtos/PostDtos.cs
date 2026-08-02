namespace Blog.Api.Dtos;

public record TagDto(string Name, string Slug);

public record PostSummaryDto(int Id, string Title, string Slug, DateTime CreatedAt, DateTime UpdatedAt, bool Published, IReadOnlyList<TagDto> Tags);

public record PostDto(int Id, string Title, string Slug, string Content, DateTime CreatedAt, DateTime UpdatedAt, bool Published, IReadOnlyList<TagDto> Tags);

public record PagedResult<T>(IReadOnlyList<T> Items, int Total, int Page, int PageSize);

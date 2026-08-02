using System.ComponentModel.DataAnnotations;

namespace Blog.Api.Models;

public class Tag
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(60)]
    public string Slug { get; set; } = string.Empty;

    public ICollection<Post> Posts { get; set; } = new List<Post>();
}

using System.ComponentModel.DataAnnotations;

namespace VozUniversitaria.Models;

public class Publication
{
    public int Id { get; set; }

    [Required]
    [StringLength(20)]
    public required string CategoryKey { get; set; }

    [Required]
    [StringLength(120)]
    public required string Title { get; set; }

    [Required]
    [StringLength(2000)]
    public required string Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int AuthorId { get; set; }

    public Users Author { get; set; } = null!;
}

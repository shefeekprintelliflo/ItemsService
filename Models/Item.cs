using System.ComponentModel.DataAnnotations;

namespace ItemsService.Models;

public class Item
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }
}

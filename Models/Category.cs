using System.ComponentModel.DataAnnotations;

namespace BookStore.Models;

public class Category : BaseEntity
{
    [StringLength(150)]
    public required string Name { get; set; }

    public string? Description { get; set; }

    public virtual List<Book> Books { get; set; } = new List<Book>();
}

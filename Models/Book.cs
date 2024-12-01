using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Models;

public class Book : BaseEntity
{
    [StringLength(150)]
    public required string Title { get; set; }

    [Column(TypeName = "money")]
    public decimal Price { get; set; }

    public int Stock { get; set; }

    [Column(TypeName = "date")]
    public DateOnly PublishDate { get; set; }

    [Column("photo")]
    public string? PhotoId { get; set; }

    [ForeignKey("Author")]
    public int AuthorId { get; set; }

    [ForeignKey("Category")]
    public int? CategoryId { get; set; }

    public virtual Author? Author { get; set; }
    public virtual Category? Category { get; set; }
    public virtual List<OrderDetails> Details { get; set; } = new List<OrderDetails>();
}

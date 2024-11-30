using BookStore.Utils;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Models;

public class Order : BaseEntity
{
    [Column(TypeName = "date")]
    public DateOnly OrderDate { get; set; }

    [Column(TypeName = "money")]
    public decimal TotalPrice { get; set; }

    public Status Status { get; set; }

    [ForeignKey("Customer")]
    public required string CustomerId { get; set; }

    public virtual Customer? Customer { get; set; }
    public virtual List<OrderDetails> Details { get; set; } = new List<OrderDetails>();
}

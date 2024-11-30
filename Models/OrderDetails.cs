using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Models;

public class OrderDetails
{
    [ForeignKey("Order")]
    public int OrderId { get; set; }

    [ForeignKey("Book")]
    public int BookId { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "money")]
    public decimal UnitPrice { get; set; }

    public virtual Order? Order { get; set; }
    public virtual Book? Book { get; set; }
}

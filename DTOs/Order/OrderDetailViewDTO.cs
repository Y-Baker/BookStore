using System;

namespace BookStore.DTOs.Order;

public class OrderDetailViewDTO
{
    public int BookId { get; set; }
    public string? BookTitle { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

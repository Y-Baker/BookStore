using System;
using BookStore.Utils;

namespace BookStore.DTOs.Order;

public class OrderViewDTO
{
    public string? CustomerName { get; set; }
    public DateOnly OrderDate { get; set; }
    public decimal TotalPrice { get; set; }
    public Status Status { get; set; }

    public List<ViewOrderDetailDTO> Details { get; set; } = new List<ViewOrderDetailDTO>();
}

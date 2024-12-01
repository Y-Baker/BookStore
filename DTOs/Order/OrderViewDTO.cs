using System;
using BookStore.Utils;

namespace BookStore.DTOs.Order;

public class OrderViewDTO
{
    public int Id { get; set; }
    public string? CustomerName { get; set; }
    public DateOnly OrderDate { get; set; }
    public decimal TotalPrice { get; set; }
    public string? Status { get; set; }

    public List<OrderDetailViewDTO> Details { get; set; } = new List<OrderDetailViewDTO>();
}

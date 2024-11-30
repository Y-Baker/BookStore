using System;
using System.ComponentModel.DataAnnotations;

namespace BookStore.DTOs.Order;

public class OrderDetailDTO
{
    [Required]
    public int BookId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}

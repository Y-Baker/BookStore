using System;
using System.ComponentModel.DataAnnotations;

namespace BookStore.DTOs.Book;

public class EditBookDTO
{
    [StringLength(150)]
    public required string Title { get; set; }

    public decimal price { get; set; }

    [Range(1,999, ErrorMessage ="Stock must be between 1 and 999")]
    public int stock { get; set; }

    [DataType(DataType.Date)]
    public DateOnly publishdate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public IFormFile? Photo { get; set; }
    public int AuthorId { get; set; }
    public int? CategoryId { get; set; }
}

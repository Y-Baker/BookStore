using System;
using System.Text.Json.Serialization;

namespace BookStore.DTOs.Book;

public class BookViewDTO
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public decimal Price { get; set; }
    public int Stock { get; set; }

    public DateOnly PublishDate { get; set; }

    public string? AuthorName { get; set; }
    public string? Category { get; set; }
}

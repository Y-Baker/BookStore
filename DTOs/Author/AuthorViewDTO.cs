using System;

namespace BookStore.DTOs.Author;

public class AuthorViewDTO
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Bio { get; set; }

    public int NumberOfBooks { get; set; }
}

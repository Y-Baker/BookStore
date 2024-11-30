using System;
using System.ComponentModel.DataAnnotations;

namespace BookStore.DTOs.Author;

public class EditAuthorDTO
{
    [StringLength(150)]
    public required string Name { get; set; }
    
    public string? Bio { get; set; }

    public int NumberOfBooks { get; set; }
}
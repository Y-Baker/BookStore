using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Models;

public class Author : BaseEntity
{
    [StringLength(150)]
    public required string Name { get; set; }

    public string? Bio { get; set; }

    public int NumberOfBooks { get; set; } = 0;

    public virtual List<Book> Books { get; set; } = new List<Book>();
}

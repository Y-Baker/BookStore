using System;

namespace BookStore.DTOs.Category;

public class CategoryViewDTO
{
    public int Id { get; set; }
    public string? Name { get; set; }

    public string? Description { get; set; }
}

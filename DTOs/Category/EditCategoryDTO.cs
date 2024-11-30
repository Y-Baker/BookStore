using System;
using System.ComponentModel.DataAnnotations;

namespace BookStore.DTOs.Category;

public class EditCategoryDTO
{
    [StringLength(150)]
    public required string Name { get; set; }
    public string? Description { get; set; }
}

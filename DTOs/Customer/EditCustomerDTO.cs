using System.ComponentModel.DataAnnotations;

namespace BookStore.DTOs.User;

public class EditCustomerDTO
{
    public required string Id { get; set; }

    [StringLength(256)]
    [RegularExpression("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$")]
    public required string Email { get; set; }

    [StringLength(256)]
    public string? FullName { get; set; }

    [StringLength(256)]
    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }
}

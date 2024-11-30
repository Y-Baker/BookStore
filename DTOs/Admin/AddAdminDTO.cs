using System.ComponentModel.DataAnnotations;

namespace BookStore.DTOs.User;

public class AddAdminDTO
{
    [StringLength(256)]
    public required string Username { get; set; }

    [StringLength(256)]
    [RegularExpression("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$", ErrorMessage = "Not a valid email")]
    public required string Email { get; set; }

    [StringLength(256)]
    [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$", ErrorMessage = "Not Strong Password")]
    public required string Password { get; set; }


    [StringLength(256)]
    public string? PhoneNumber { get; set; }
}

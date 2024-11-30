using System.ComponentModel.DataAnnotations;

namespace BookStore.DTOs.User;

public class ChangePasswordDTO
{
    [StringLength(256)]
    public required string OldPassword { get; set; }

    [StringLength(256)]
    [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$", ErrorMessage = "Not Strong Password")]
    public required string NewPassword { get; set; }

    [Compare("NewPassword", ErrorMessage = "Passwords doesn't matched")]
    public required string ConfirmPassword { get; set; }
}

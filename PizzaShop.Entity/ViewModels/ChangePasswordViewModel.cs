using System.ComponentModel.DataAnnotations;
using PizzaShop.Entity.Models;

namespace PizzaShop.Entity.ViewModels
{
    public class ChangePasswordViewModel
    {
        [EmailAddress]
        public string? Email { get; set; }

        // public string Token { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string? OldPassword { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]    
        [DataType(DataType.Password)]    
        [RegularExpression("^(?=.*[A-Za-z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$",ErrorMessage = "Password must contain at least one number and one uppercase and lowercase letter, and at least 8 or more characters")]   
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }
    }
}
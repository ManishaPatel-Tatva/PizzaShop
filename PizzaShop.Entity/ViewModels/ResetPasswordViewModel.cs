using System.ComponentModel.DataAnnotations;

namespace PizzaShop.Entity.ViewModels
{
    public class ResetPasswordViewModel
    {
        public string? Token { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]    
        [DataType(DataType.Password)]    
        [RegularExpression("^(?=.*[A-Za-z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$",ErrorMessage = "Password must contain at least one number and one uppercase and lowercase letter, and at least 8 or more characters")]   
        public required string NewPassword { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [RegularExpression("^(?=.*[A-Za-z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$",ErrorMessage = "Password must contain at least one number and one uppercase and lowercase letter, and at least 8 or more characters")]   
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public required string ConfirmPassword { get; set; }
    }
}
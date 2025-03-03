using System.ComponentModel.DataAnnotations;

namespace PizzaShop.Service.ViewModels
{
    public class ResetPasswordViewModel
    {
        public string? Token { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]    
        [DataType(DataType.Password)]    
        [MinLength(8, ErrorMessage = "Minimum 8 character required")]    
        public required string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public required string ConfirmPassword { get; set; }
    }
}
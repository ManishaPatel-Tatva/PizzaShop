using System.ComponentModel.DataAnnotations;

namespace PizzaShop.ViewModel
{
    public class ResetPasswordViewModel
    {
        public string? Email { get; set; }

        // public string Token { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }
    }
}
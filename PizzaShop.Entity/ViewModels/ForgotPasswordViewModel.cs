using System.ComponentModel.DataAnnotations;
namespace PizzaShop.Entity.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email ID is required")]    
        [EmailAddress]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid Email Format")]
        public required string Email { get; set; }   
    }
}
using System.ComponentModel.DataAnnotations;
namespace PizzaShop.Entity.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email ID is required")]    
        [EmailAddress]
        public required string Email { get; set; }   
    }
}
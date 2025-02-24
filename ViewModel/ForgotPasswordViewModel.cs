using System.ComponentModel.DataAnnotations;
using PizzaShop.Models;

namespace PizzaShop.ViewModel
{
    public class ForgotPasswordViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email ID is required")]    
        [EmailAddress]
        public string Email { get; set; }   
    }
}
using System.ComponentModel.DataAnnotations;

namespace PizzaShop.Service.ViewModels
{
    public class LoginViewModel
    {

        [Required(AllowEmptyStrings = false, ErrorMessage = "Email ID is required")]    
        [EmailAddress]
        public required string Email { get; set; }   

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]    
        [DataType(DataType.Password)]    
        public required string Password { get; set; }    

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }  
        
    }
}

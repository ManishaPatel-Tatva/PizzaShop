using System.ComponentModel.DataAnnotations;

namespace PizzaShop.Entity.ViewModels
{
    public class LoginViewModel
    {

        [Required(AllowEmptyStrings = false, ErrorMessage = "Email ID is required")]    
        [EmailAddress]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid Email Format")]
        public required string Email { get; set; }   

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]    
        [DataType(DataType.Password)]    
        // [RegularExpression("^(?=.*[A-Za-z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$",ErrorMessage = "Password must contain at least one number and one uppercase and lowercase letter, and at least 8 or more characters")]
        public required string Password { get; set; }    

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }  
        
    }
}

using System.ComponentModel.DataAnnotations;
using DataAccessLayer.Models;

namespace DataAccessLayer.ViewModels
{
    public class LoginViewModel
    {

        [Required(AllowEmptyStrings = false, ErrorMessage = "Email ID is required")]    
        [EmailAddress]
        public string Email { get; set; }   

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]    
        [DataType(DataType.Password)]    
        [MinLength(8, ErrorMessage = "Minimum 8 character required")]    
        public string Password { get; set; }    

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }  
        
    }
}

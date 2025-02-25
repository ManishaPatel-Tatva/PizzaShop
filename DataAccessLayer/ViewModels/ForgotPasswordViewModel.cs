using System.ComponentModel.DataAnnotations;
using DataAccessLayer.Models;
namespace DataAccessLayer.ViewModel
{
    public class ForgotPasswordViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email ID is required")]    
        [EmailAddress]
        public string Email { get; set; }   
    }
}
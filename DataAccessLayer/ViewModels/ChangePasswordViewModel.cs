using System.ComponentModel.DataAnnotations;
using DataAccessLayer.Models;

namespace DataAccessLayer.ViewModel
{
    public class ChangePasswordViewModel
    {
        [EmailAddress]
        public string? Email { get; set; }

        // public string Token { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string? OldPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }
    }
}
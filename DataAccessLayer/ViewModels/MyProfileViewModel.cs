using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using DataAccessLayer.Models;

namespace DataAccessLayer.ViewModel{
    public class MyProfileViewModel
    {
        // [Required(ErrorMessage = "First Name is required")]
        // [StringLength(50, ErrorMessage = "First Name cannot exceed 50 characters")]
        public string FirstName { get; set; } = null!;

        // [Required(ErrorMessage = "Last Name is required")]
        // [StringLength(50, ErrorMessage = "Last Name cannot exceed 50 characters")]
        public string LastName { get; set; } = null!;

        // [Required(ErrorMessage = "User Name is required")]
        // [StringLength(50, ErrorMessage = "User Name cannot exceed 50 characters")]
        public string UserName { get; set; } = null!;

        public long Phone { get; set; } 

        // [Required(ErrorMessage = "Email is required")]
        // [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; } = null!;

        // [Required(ErrorMessage = "Address is required")]
        // [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string Address { get; set; } = null!;

        // [Required(ErrorMessage = "Zip Code is required")]
        // [RegularExpression(@"^\d{5,6}$", ErrorMessage = "Invalid Zip Code")]
        public int ZipCode { get; set; }

        public string ProfileImageUrl { get; set; } = "~/images/Default_pfp.svg.png";

        public IFormFile? image {get; set;} = null!;

        public long CountryId {get; set;}
        public long StateId {get; set;}
        public long CityId {get; set;}
        
        public string? Role { get; set; } =null!;
    }
}



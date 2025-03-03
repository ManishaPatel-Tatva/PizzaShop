using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using PizzaShop.Entity.Models;

namespace PizzaShop.Entity.ViewModels
{
    public class AddUserViewModel
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

        // [Required(ErrorMessage = "Email is required")]
        // [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = null!;

        // [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]    
        // [DataType(DataType.Password)]    
        // [MinLength(8, ErrorMessage = "Minimum 8 character required")]    
        public string Password { get; set; } = null!;

        // [Required(ErrorMessage = "Address is required")]
        // [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string Address { get; set; } = null!;

        // [Required(ErrorMessage = "Zip Code is required")]
        // [RegularExpression(@"^\d{5,6}$", ErrorMessage = "Invalid Zip Code")]
        public int ZipCode { get; set; }

        public long Phone { get; set; } 

        public string ProfileImageUrl { get; set; } = "~/images/Default_pfp.svg.png";
        public IFormFile? Image {get; set;} = null!;

        public long CountryId {get; set;}
        public long StateId {get; set;}
        public long CityId {get; set;}
        public List<Country> Countries {get; set;} = new List<Country>();
        public List<State> States {get; set;} = new List<State>();
        public List<City> Cities {get; set;} = new List<City>();

        public long RoleId { get; set; }
        public List<Role> Roles {get; set;} = new List<Role>();
    
    }
}
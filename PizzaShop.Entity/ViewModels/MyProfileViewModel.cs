using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using PizzaShop.Entity.Models;

namespace PizzaShop.Entity.ViewModels{
    public class MyProfileViewModel
    {
        [Required(ErrorMessage = "First Name is required")]
        [StringLength(50, ErrorMessage = "First Name cannot exceed 50 characters")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(50, ErrorMessage = "Last Name cannot exceed 50 characters")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "User Name is required")]
        [StringLength(50, ErrorMessage = "User Name cannot exceed 50 characters")]
        public string UserName { get; set; } = null!;


        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits.")]
        public long Phone { get; set; } 

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid Email Format")]
        public string? Email { get; set; } = null!;

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string Address { get; set; } = null!;

        [Required(ErrorMessage = "Zip Code is required")]
        [RegularExpression(@"^\d{5,6}$", ErrorMessage = "Invalid Zip Code")]
        public int ZipCode { get; set; }

        public string? ProfileImageUrl { get; set; } = "/images/Default_pfp.svg.png";

        public IFormFile? Image {get; set;} = null!;

        public long CountryId {get; set;}
        public long StateId {get; set;}
        public long CityId {get; set;}
        
        public string? Role { get; set; } =null!;

        public List<Country> Countries {get; set;} = new List<Country>();
        public List<State> States {get; set;} = new List<State>();
        public List<City> Cities {get; set;} = new List<City>();
    }
}



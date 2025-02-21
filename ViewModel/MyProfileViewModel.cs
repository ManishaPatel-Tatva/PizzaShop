using System.ComponentModel.DataAnnotations;
using PizzaShop.Models;

namespace PizzaShop.ViewModel;

using System.ComponentModel.DataAnnotations;

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

    public long CountryId {get; set;}
    public long StateId {get; set;}
    public long CityId {get; set;}
    
    // [Required(ErrorMessage = "Country is required")]
    public List<Country> Countries { get; set; } = new List<Country>();

    // [Required(ErrorMessage = "State is required")]
    public List<State> States { get; set; }  = new List<State>();

    // [Required(ErrorMessage = "City is required")]
    public List<City> Cities { get; set; }  = new List<City>();
    
    public string? Role { get; set; } =null!;
}
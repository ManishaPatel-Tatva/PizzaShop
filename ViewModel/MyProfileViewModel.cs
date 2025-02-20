using System.ComponentModel.DataAnnotations;
using PizzaShop.Models;

namespace PizzaShop.ViewModel;

using System.ComponentModel.DataAnnotations;

public class MyProfileViewModel
{
    [Required(ErrorMessage = "First Name is required")]
    [StringLength(50, ErrorMessage = "First Name cannot exceed 50 characters")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last Name is required")]
    [StringLength(50, ErrorMessage = "Last Name cannot exceed 50 characters")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "User Name is required")]
    [StringLength(50, ErrorMessage = "User Name cannot exceed 50 characters")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required")]
    [Phone(ErrorMessage = "Invalid phone number format")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Country is required")]
    public string Country { get; set; } = "India";

    [Required(ErrorMessage = "State is required")]
    public string State { get; set; } = string.Empty;

    [Required(ErrorMessage = "City is required")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "Address is required")]
    [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "Zip Code is required")]
    [RegularExpression(@"^\d{5,6}$", ErrorMessage = "Invalid Zip Code")]
    public string ZipCode { get; set; } = string.Empty;

    public string ProfileImageUrl { get; set; } = "~/images/Default_pfp.svg.png";
}
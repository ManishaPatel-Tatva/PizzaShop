using System.ComponentModel.DataAnnotations;

namespace PizzaShop.Entity.ViewModels;

public class WaitingTokenViewModel
{
    public long Id { get; set; } = 0;
    public long CustomerId { get; set; } = 0;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid Email Format")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Name is required")]
    [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Phone number is required")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits.")]
    public long Phone { get; set; }

    [Required(ErrorMessage = "Member is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Member should be greater than 0.")]
    public int Members { get; set; } = 0;

    public long SectionId { get; set; }
    public List<SectionViewModel> Sections { get; set; } = new List<SectionViewModel>();

    public DateTime CreatedAt { get; set; }

}

using System.ComponentModel.DataAnnotations;

namespace PizzaShop.Entity.ViewModels;

public class SectionViewModel
{
    public long Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Description is required")]
    public string? Description { get; set; } = null!;
    public List<TableCardViewModel>? Tables { get; set; } = new List<TableCardViewModel>();
}

using System.ComponentModel.DataAnnotations;

namespace PizzaShop.Entity.ViewModels;

public class CategoryViewModel
{
    public long Id { get; set; } = 0;

    [Required(ErrorMessage = "Category Name is required")]
    [StringLength(50, ErrorMessage = "Category Name cannot exceed 50 characters")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Description is required")]
    public string? Desc { get; set; }
}

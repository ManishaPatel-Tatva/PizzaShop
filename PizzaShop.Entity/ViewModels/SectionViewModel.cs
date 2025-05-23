using System.ComponentModel.DataAnnotations;

namespace PizzaShop.Entity.ViewModels;

public class SectionViewModel
{
    public long Id { get; set; } = 0;

    [Required(ErrorMessage = "Name is required")]
    [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Description is required")]
    public string? Description { get; set; } = "";
    public List<TableCardViewModel> Tables { get; set; } = new();
    public int TokenCount { get; set; } = 0;
}

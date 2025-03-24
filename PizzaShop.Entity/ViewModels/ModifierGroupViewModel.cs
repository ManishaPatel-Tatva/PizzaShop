using System.ComponentModel.DataAnnotations;
using PizzaShop.Entity.Models;

namespace PizzaShop.Entity.ViewModels;

public class ModifierGroupViewModel
{
    public long ModifierGroupId { get; set; } = 0;

    [Required(ErrorMessage = "Name is required")]
    [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Description is required")]
    public string? Description { get; set; } = null!;

    public List<ModifierInfoViewModel> Modifiers { get; set; } = new List<ModifierInfoViewModel>();
    
    public List<long>? ModifierIdList { get; set; } = new List<long>();
}

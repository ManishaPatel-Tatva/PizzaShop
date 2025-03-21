using PizzaShop.Entity.Models;

namespace PizzaShop.Entity.ViewModels;

public class ModifierGroupViewModel
{
    public long ModifierGroupId { get; set; } = 0;

    public string? Name { get; set; }

    public string? Description { get; set; } = null!;

    public List<ModifierInfoViewModel> Modifiers { get; set; } = new List<ModifierInfoViewModel>();
    
    public List<long>? ModifierIdList { get; set; } = new List<long>();
}

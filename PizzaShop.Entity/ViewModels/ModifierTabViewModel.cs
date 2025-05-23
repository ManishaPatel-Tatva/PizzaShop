using PizzaShop.Entity.Models;

namespace PizzaShop.Entity.ViewModels;

public class ModifierTabViewModel
{
    public List<ModifierGroupViewModel> ModifierGroups { get; set; } = new();
    public List<Modifier> Modifiers { get; set; } = new List<Modifier>();
}

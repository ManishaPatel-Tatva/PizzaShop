using PizzaShop.Entity.Models;

namespace PizzaShop.Entity.ViewModels;

public class ModifierGroupViewModel
{
    public long ModifierGroupId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public List<ModifierGroup> ModifierGroups { get; set; } = new List<ModifierGroup>();

    public List<Modifier> Modifiers { get; set; } = new List<Modifier>();
}

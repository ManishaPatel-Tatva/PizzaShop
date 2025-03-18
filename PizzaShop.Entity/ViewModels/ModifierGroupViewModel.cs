using PizzaShop.Entity.Models;

namespace PizzaShop.Entity.ViewModels;

public class ModifierGroupViewModel
{
    public long ModifierGroupId { get; set; }

    public string Name { get; set; }

    public string? Description { get; set; } = null!;

}

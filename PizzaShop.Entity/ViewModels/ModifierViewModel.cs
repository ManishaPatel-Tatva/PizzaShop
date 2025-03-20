using PizzaShop.Entity.Models;

namespace PizzaShop.Entity.ViewModels;

public class ModifierViewModel
{
    public long ModifierGroupId { get; set; } = 0;
    public long ModifierId { get; set;} = 0;
    public string ModifierName { get; set;}
    public long UnitId { get; set; }
    public string? UnitName { get; set; } = null!;
    public decimal Rate { get; set; }
    public int Quantity { get; set; }
    public string Description { get; set; } = null!;

    public List<ModifierGroup>? ModifierGroups { get; set; } = new List<ModifierGroup>();
    public List<Unit>? Units { get; set; } = new List<Unit>();

}

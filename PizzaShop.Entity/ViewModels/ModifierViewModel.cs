namespace PizzaShop.Entity.ViewModels;

public class ModifierViewModel
{
    public long ModifierId { get; set;}

    public string ModifierName { get; set;}

    public string Unit { get; set; } = null!;

    public decimal Rate { get; set; }

    public decimal Quantity { get; set; }

}

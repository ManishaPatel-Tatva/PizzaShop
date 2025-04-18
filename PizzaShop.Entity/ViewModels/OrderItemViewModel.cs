namespace PizzaShop.Entity.ViewModels;

public class OrderItemViewModel
{
    public long ItemId { get; set;} = 0;
    public string ItemName { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal TotalAmount { get; set; }
    public List<ModifierViewModel> ModifiersList { get; set; } = new List<ModifierViewModel>();
    public string Instruction { get; set; } = "";
    public bool IsSelected { get; set; } = false;
}

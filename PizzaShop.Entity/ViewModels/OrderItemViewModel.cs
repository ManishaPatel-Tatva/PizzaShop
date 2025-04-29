namespace PizzaShop.Entity.ViewModels;

public class OrderItemViewModel
{
    public long Id { get; set;} = 0;
    public string Name { get; set; } ="";
    public int Quantity { get; set; } = 1;
    public decimal Price { get; set; }
    public decimal TotalAmount { get; set; }
    public List<ModifierViewModel> ModifiersList { get; set; } = new List<ModifierViewModel>();
    public string? Instruction { get; set; } 
    public bool IsSelected { get; set; } = false;
    public int ReadyQuantity { get; set; } = 0;
}

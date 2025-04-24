namespace PizzaShop.Entity.ViewModels;

public class ItemInfoViewModel
{
    public long Id { get; set; }
    public string ImageUrl { get; set; } = "";
    public required string Name { get; set; }
    public string Type { get; set; } = "";
    public decimal Rate { get; set; }
    public int Quantity { get; set; }
    public bool Available { get; set; }
    public bool IsFavourite { get; set; } = false;
}

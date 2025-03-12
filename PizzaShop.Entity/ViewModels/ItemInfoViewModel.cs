namespace PizzaShop.Entity.ViewModels;

public class ItemInfoViewModel
{
    public long ItemId { get; set; }
    public string ItemImageUrl { get; set; }

    public required string ItemName { get; set; }

    public string ItemType { get; set; }

    public decimal Rate { get; set; }

    public int Quantity { get; set; }

    public bool Available { get; set; }

}

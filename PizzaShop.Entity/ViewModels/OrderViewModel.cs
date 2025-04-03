namespace PizzaShop.Entity.ViewModels;

public class OrderViewModel
{
    public long OrderId { get; set; }
    public DateOnly Date { get; set; }
    public string CustomerName { get; set; }
    public string Status { get; set; }
    public string PaymentMode { get; set; }
    public int Rating { get; set; }
    public decimal TotalAmount { get; set; }
    public bool? IsDineIn { get; set; } = true;
    public int NoOfItems { get; set; }
}

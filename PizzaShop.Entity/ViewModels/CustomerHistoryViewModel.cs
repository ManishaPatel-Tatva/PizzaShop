namespace PizzaShop.Entity.ViewModels;

public class CustomerHistoryViewModel
{
    public long CustomerId { get; set; }
    public string CustomerName { get; set; }
    public long Phone { get; set; }
    public decimal MaxOrder { get; set; }
    public decimal AvgBill { get; set; }
    public DateTime ComingSince  { get; set; }
    public int Visits { get; set; }
    public List<OrderViewModel> Orders { get; set; } = new List<OrderViewModel>();
}

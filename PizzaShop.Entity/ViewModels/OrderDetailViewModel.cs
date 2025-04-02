namespace PizzaShop.Entity.ViewModels;

public class OrderDetailViewModel
{
    public long OrderId { get; set; }
    public string OrderStatus { get; set; }
    public string InvoiceNo { get; set; }
    public string PaidOn { get; set; }
    public string PlacedOn { get; set; }
    public string ModifiedOn { get; set; }
    public string OrderDuration { get; set; }
    public string CustomerName { get; set; }
    public long CustomerPhone { get; set; }
    public int NoOfPerson { get; set; }
    public string CustomerEmail { get; set; }
    public List<string> TableList { get; set; }
    public string Section { get; set; }
    public List<OrderItemViewModel> ItemsList { get; set; } = new List<OrderItemViewModel>();
    public decimal Subtotal { get; set; }
    public List<TaxViewModel> TaxList { get; set; } = new List<TaxViewModel>();
    public decimal FinalAmount { get; set; }
    public string PaymentMethod { get; set; }
}

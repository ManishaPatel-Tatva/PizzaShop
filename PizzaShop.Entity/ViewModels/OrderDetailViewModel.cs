namespace PizzaShop.Entity.ViewModels;

public class OrderDetailViewModel
{
    public long OrderId { get; set; } = 0;
    public string OrderStatus { get; set; } = "";
    public string InvoiceNo { get; set; } = "";
    public string PaidOn { get; set; } = "";
    public string PlacedOn { get; set; } = "";
    public string ModifiedOn { get; set; } = "";
    public string OrderDuration { get; set; } = "";
    public long CustomerId { get; set; } = 0;
    public string CustomerName { get; set; } = "";
    public long CustomerPhone { get; set; }
    public int NoOfPerson { get; set; } = 1;
    public string CustomerEmail { get; set; } = "";
    public List<string> TableList { get; set; } = new();
    public string Section { get; set; } = "";
    public List<OrderItemViewModel> ItemsList { get; set; } = new();
    public decimal Subtotal { get; set; }
    public List<TaxViewModel> TaxList { get; set; } = new();
    public List<long> Taxes { get; set; } = new();
    public decimal FinalAmount { get; set; }
    public long PaymentMethodId { get; set; } =  1;
    public string PaymentMethod { get; set; } = "";
    public string? Comment { get; set; } 
}

using PizzaShop.Entity.Models;

namespace PizzaShop.Entity.ViewModels;

public class AppMenuViewModel
{
    public List<CategoryViewModel> Categories { get; set; } = new ();
    public long CustomerId { get; set; } = 0;
    public string SectionName { get; set; } = "";
    public List<string> Tables { get; set; } = new();
    public long OrderId { get; set; } = 0;
    public OrderDetailViewModel Order { get; set; } = new();
    public List<Taxis> Taxes { get; set; } = new ();
    public List<PaymentMethod> PaymentMethods { get; set; } = new();
}

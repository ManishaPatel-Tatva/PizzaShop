namespace PizzaShop.Entity.ViewModels;

public class OrderPaginationViewModel
{
    public IEnumerable<OrderViewModel> Orders { get; set; } = new List<OrderViewModel>();
    public Pagination Page { get; set; } = new ();
}

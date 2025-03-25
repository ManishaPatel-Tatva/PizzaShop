namespace PizzaShop.Entity.ViewModels;

public class OrderPaginationViewModel
{
    public IEnumerable<OrderViewModel>? Orders { get; set; }
    public Pagination? Page { get; set; }
}

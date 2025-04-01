namespace PizzaShop.Entity.ViewModels;

public class CustomerPaginationViewModel
{
    public IEnumerable<CustomerViewModel>? Customers { get; set; }
    public Pagination? Page { get; set; }
}

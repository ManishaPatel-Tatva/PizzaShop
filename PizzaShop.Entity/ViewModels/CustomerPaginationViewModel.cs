namespace PizzaShop.Entity.ViewModels;

public class CustomerPaginationViewModel
{
    public IEnumerable<CustomerViewModel> Customers { get; set; } = new List<CustomerViewModel>();
    public Pagination Page { get; set; } = new();
}

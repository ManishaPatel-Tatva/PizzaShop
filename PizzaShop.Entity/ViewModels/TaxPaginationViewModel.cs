namespace PizzaShop.Entity.ViewModels;

public class TaxPaginationViewModel
{
    public IEnumerable<TaxViewModel> Taxes { get; set; } = new List<TaxViewModel>();
    public Pagination Page { get; set; } = new();
}

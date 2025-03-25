namespace PizzaShop.Entity.ViewModels;

public class TaxPaginationViewModel
{
    public IEnumerable<TaxViewModel>? Taxes { get; set; }
    public Pagination? Page { get; set; }
}

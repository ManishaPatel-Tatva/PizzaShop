namespace PizzaShop.Entity.ViewModels;

public class TablesPaginationViewModel
{
    public IEnumerable<TableViewModel>? Tables { get; set; }
    public Pagination? Page { get; set; }
}

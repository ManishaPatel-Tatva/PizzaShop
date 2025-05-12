namespace PizzaShop.Entity.ViewModels;

public class TablesPaginationViewModel
{
    public IEnumerable<TableViewModel> Tables { get; set; } = new List<TableViewModel>();
    public Pagination Page { get; set; } = new();
}

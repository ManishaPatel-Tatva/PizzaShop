namespace PizzaShop.Entity.ViewModels;

public class ItemsPaginationViewModel
{
    public IEnumerable<ItemInfoViewModel>? Items { get; set; }
    public Pagination? Page { get; set; }
}

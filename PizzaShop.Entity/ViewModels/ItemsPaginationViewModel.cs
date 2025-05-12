namespace PizzaShop.Entity.ViewModels;

public class ItemsPaginationViewModel
{
    public IEnumerable<ItemInfoViewModel> Items { get; set; } = new List<ItemInfoViewModel>();
    public Pagination Page { get; set; } = new();
}

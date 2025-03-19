namespace PizzaShop.Entity.ViewModels;

public class ModifiersPaginationViewModel
{
    public IEnumerable<ModifierViewModel>? Modifiers { get; set; }
    public Pagination? Page { get; set; }
}

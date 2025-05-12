namespace PizzaShop.Entity.ViewModels;

public class ModifiersPaginationViewModel
{
    public IEnumerable<ModifierViewModel> Modifiers { get; set; } = new List<ModifierViewModel>();
    public Pagination Page { get; set; }  = new();
}

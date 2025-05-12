namespace PizzaShop.Entity.ViewModels;

public class MenuViewModel
{
    public List<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();
    public CategoryViewModel CategoryVM { get; set; } = new();
    public ItemsPaginationViewModel ItemsPageVM {get; set;} = new();

}

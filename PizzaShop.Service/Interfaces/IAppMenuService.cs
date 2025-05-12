using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IAppMenuService
{
    Task<AppMenuViewModel> Get(long customerId);
    Task<List<ItemInfoViewModel>> List(long categoryId, string search);
}

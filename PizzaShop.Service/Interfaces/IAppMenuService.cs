using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IAppMenuService
{
    Task<List<ItemInfoViewModel>> Get(long categoryId, string search);
    Task<ResponseViewModel> FavouriteItem(long itemId);
}

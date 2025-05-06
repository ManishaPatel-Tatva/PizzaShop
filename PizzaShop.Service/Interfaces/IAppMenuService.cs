using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IAppMenuService
{
    Task<AppMenuViewModel> Get(long customerId);
    Task<List<ItemInfoViewModel>> List(long categoryId, string search);
    Task<ResponseViewModel> FavouriteItem(long itemId);
    Task<ResponseViewModel> Save(OrderDetailViewModel orderVM);
    Task<bool> SaveOrderItemModifier(ModifierViewModel modifier, long orderItemId);
    Task<ResponseViewModel> CompleteOrder(long orderId);
    Task<ResponseViewModel> CancelOrder(long orderId);
}

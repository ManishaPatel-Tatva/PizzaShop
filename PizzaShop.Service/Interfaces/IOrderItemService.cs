using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IOrderItemService
{
    Task<bool> Save(List<OrderItemViewModel> items, long orderId);
    decimal OrderItemTotal(long orderId);
}

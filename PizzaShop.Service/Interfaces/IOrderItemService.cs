using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IOrderItemService
{
    Task Save(OrderItemViewModel orderItemVM, long ordserId);
    Task Save(List<OrderItemViewModel> items, long orderId);
    Task<decimal> OrderItemTotal(long orderId);
    Task Delete(long orderItemId);
}

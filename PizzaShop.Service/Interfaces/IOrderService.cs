using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IOrderService
{
    Task<OrderIndexViewModel> GetOrderIndex();
    Task<OrderPaginationViewModel> GetPagedOrder(int pageSize, int pageNumber, string search);

}

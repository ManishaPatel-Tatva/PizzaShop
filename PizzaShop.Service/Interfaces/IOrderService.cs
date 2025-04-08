using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IOrderService
{
    Task<OrderIndexViewModel> Get();
    Task<OrderPaginationViewModel> Get(FilterViewModel filter);
    Task<OrderDetailViewModel> Get(long orderId);
    Task<IEnumerable<Order>> List(FilterViewModel filter);
    Task<byte[]> ExportExcel(FilterViewModel filter);
}

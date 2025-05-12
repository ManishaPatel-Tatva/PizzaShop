using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IOrderService
{
    OrderIndexViewModel Get();
    Task<OrderPaginationViewModel> Get(FilterViewModel filter);
    Task<OrderDetailViewModel> Get(long orderId);
    Task<IEnumerable<Order>> List(FilterViewModel filter);
    Task<byte[]> ExportExcel(FilterViewModel filter);
    Task ChangeStatus(long orderId, string status);
    Task<ResponseViewModel> Save(OrderDetailViewModel orderVM);
    Task<ResponseViewModel> CompleteOrder(long orderId);
    Task<ResponseViewModel> CancelOrder(long orderId);

}

using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IOrderService
{
    Task<OrderIndexViewModel> Get();
    Task<OrderPaginationViewModel> Get(FilterViewModel filter);
    Task<OrderDetailViewModel> Get(long orderId);
    Task<byte[]> ExportExcel(string status, string dateRange, DateOnly? fromDate, DateOnly? toDate, string column, string sort, string search);
}

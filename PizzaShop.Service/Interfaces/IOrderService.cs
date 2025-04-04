using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IOrderService
{
    Task<OrderIndexViewModel> GetOrderIndex();
    Task<OrderPaginationViewModel> GetPagedRecord(FilterViewModel filter);
    Task<byte[]> ExportExcel(string status, string dateRange, DateOnly? fromDate, DateOnly? toDate, string column, string sort, string search);
    Task<OrderDetailViewModel> GetOrderDetail(long orderId);
}

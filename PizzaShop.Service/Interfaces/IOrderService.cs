using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IOrderService
{
    Task<OrderIndexViewModel> GetOrderIndex();
    Task<OrderPaginationViewModel> GetPagedOrder(string status, string dateRange, DateOnly? fromDate, DateOnly? toDate, string column, string sort, int pageSize, int pageNumber, string search);
    Task<byte[]> ExportOrderDetails(string status, string dateRange, DateOnly? fromDate, DateOnly? toDate, string column, string sort, string search);
}

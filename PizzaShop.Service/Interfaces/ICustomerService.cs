using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface ICustomerService
{
    Task<CustomerPaginationViewModel> GetPagedCustomers(string dateRange, DateOnly? fromDate, DateOnly? toDate, string column, string sort, int pageSize, int pageNumber, string search);
    Task<CustomerHistoryViewModel> CustomerHistory(long customerId);
    Task<byte[]> ExportExcel(string dateRange, DateOnly? fromDate, DateOnly? toDate, string column, string sort, string search);
}

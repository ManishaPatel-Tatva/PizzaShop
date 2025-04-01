using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface ICustomerService
{
    Task<CustomerPaginationViewModel> GetPagedCustomers(string dateRange, DateOnly? fromDate, DateOnly? toDate, string column, string sort, int pageSize, int pageNumber, string search);

}

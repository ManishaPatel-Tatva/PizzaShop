using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface ICustomerService
{
    Task<CustomerPaginationViewModel> Get(FilterViewModel filter);
    Task<CustomerHistoryViewModel> Get(long customerId);
    Task<byte[]> ExportExcel(FilterViewModel filter);
}

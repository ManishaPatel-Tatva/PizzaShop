using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface ICustomerService
{
    Task<CustomerViewModel> Get(string email);
    Task<CustomerPaginationViewModel> Get(FilterViewModel filter);
    Task<CustomerHistoryViewModel> Get(long customerId);
    Task<ResponseViewModel> Save(CustomerViewModel customerVM);
    Task<byte[]> ExportExcel(FilterViewModel filter);
}

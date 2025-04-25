using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface ICustomerService
{
    Task<CustomerViewModel> Get(string email);
    Task<CustomerViewModel> Get(long id);
    Task<CustomerPaginationViewModel> Get(FilterViewModel filter);
    Task<CustomerHistoryViewModel> GetHistory(long customerId);
    Task<ResponseViewModel> Save(CustomerViewModel customerVM);
    Task<byte[]> ExportExcel(FilterViewModel filter);
}

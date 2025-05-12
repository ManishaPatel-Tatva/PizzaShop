using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface ICustomerService
{
    Task<CustomerViewModel> Get(string email);
    CustomerViewModel Get(long id);
    Task<CustomerPaginationViewModel> Get(FilterViewModel filter);
    Task<List<CustomerViewModel>> List(FilterViewModel filter);
    CustomerHistoryViewModel GetHistory(long customerId);
    Task<ResponseViewModel> Save(CustomerViewModel customerVM);
    Task<byte[]> ExportExcel(FilterViewModel filter);
}

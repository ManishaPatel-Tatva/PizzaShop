using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface ITaxesFeesService
{
    Task<TaxPaginationViewModel> Get(FilterViewModel filter);
    Task<TaxViewModel> Get(long TaxId);
    Task<ResponseViewModel> Save(TaxViewModel model, string createrEmail);
    Task<bool> Delete(long TaxId, string createrEmail);
}

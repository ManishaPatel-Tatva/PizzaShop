using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface ITaxesFeesService
{
    Task<List<TaxViewModel>> GetAllTaxes(string search);
    Task<TaxViewModel> GetTax(long TaxId);
    Task<bool> SaveTax(TaxViewModel model, string createrEmail);
    Task<bool> AddTax(TaxViewModel model, long createrId);
    Task<bool> UpdateTax(TaxViewModel model, long createrId);
    Task<bool> DeleteTax(long TaxId, string createrEmail);
}

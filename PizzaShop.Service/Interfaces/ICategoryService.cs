using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface ICategoryService
{
    List<CategoryViewModel> Get();
    Task<CategoryViewModel> Get(long categoryId);
    Task<ResponseViewModel> Save(CategoryViewModel categoryVM,string createrEmail);
    Task<bool> Delete(long categoryId, string createrEmail);
}

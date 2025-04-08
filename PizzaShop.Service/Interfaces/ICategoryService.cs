using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface ICategoryService
{
    Task<List<CategoryViewModel>> Get();
    Task<CategoryViewModel> Get(long categoryId);
    Task<ResponseViewModel> Save(CategoryViewModel category,string createrEmail);
    Task<bool> Delete(long categoryId, string createrEmail);
}

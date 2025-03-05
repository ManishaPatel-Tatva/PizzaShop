using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface ICategoryItemService
{
    List<CategoryViewModel> GetCategory();

    CategoryViewModel GetCategoryById(long categoryId);

    Task<bool> AddCategory(CategoryViewModel model,string createrEmail);
    
    Task<bool> EditCategory(CategoryViewModel model);

    Task<bool> SoftDelete(long categoryId);

    ItemsPaginationViewModel GetItems(int pageSize, int pageNumber);


}

using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Services;

public class CategoryItemService : ICategoryItemService
{
    private readonly IGenericRepository<Category> _categoryRepository;

    public CategoryItemService(IGenericRepository<Category> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

 /*-----------------------------------------------------------------Category---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    #region View Category

    public List<CategoryViewModel> GetCategory()
    {
        var categories =  _categoryRepository.GetAll();

        var categoriesList = new List<CategoryViewModel>();
        
        foreach(var category in categories)
        {
            var categoryVM = new CategoryViewModel(){
                CategoryId = category.Id,
                CategoryName = category.Name,
                CategoryDesc = category.Description
            };

            categoriesList.Append(categoryVM);
        }
        
        return categoriesList; 
    }

    #endregion  

    #region Edit Category

    
    #endregion

}

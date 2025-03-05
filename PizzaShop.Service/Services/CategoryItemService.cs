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
        var categories = _categoryRepository.GetAll()
        .Select(category => new CategoryViewModel
        {
            CategoryId = category.Id,
            CategoryName = category.Name,
            CategoryDesc = category.Description
        }).ToList();

        return categories;
    }

    #endregion  

    #region Edit Category

    
    #endregion

}

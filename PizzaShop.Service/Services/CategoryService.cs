using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Common;
using PizzaShop.Service.Exceptions;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Services;

public class CategoryService : ICategoryService
{
    private readonly IGenericRepository<Category> _categoryRepository;
    private readonly IUserService _userService;

    public CategoryService(IGenericRepository<Category> categoryRepository, IUserService userService)
    {
        _categoryRepository = categoryRepository;
        _userService = userService;
    }

    #region Get
    /*-----------------------------------------------------------Get Categories List---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<List<CategoryViewModel>> Get()
    {
        IEnumerable<Category>? categories = await _categoryRepository.GetByCondition(
            predicate: c => !c.IsDeleted,
            orderBy: q => q.OrderBy(c => c.Id)
            );

        List<CategoryViewModel>? list = categories.Select(category => new CategoryViewModel
        {
            Id = category.Id,
            Name = category.Name,
            Desc = category.Description
        }).ToList();

        return list;
    }

    /*--------------------------------------------------------------Get Category By Id for Editing---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<CategoryViewModel> Get(long categoryId)
    {
        Category? category = await _categoryRepository.GetByIdAsync(categoryId);

        if (category == null)
        {
            return new CategoryViewModel();
        }

        CategoryViewModel categoryVM = new()
        {
            Id = category.Id,
            Name = category.Name,
            Desc = category.Description
        };

        return categoryVM;
    }
    #endregion

    #region  Save
    /*-------------------------------------------------------------Save Category---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<ResponseViewModel> Save(CategoryViewModel categoryVM)
    {
        ResponseViewModel response = new();

        Category? existingCategory = await _categoryRepository.GetByStringAsync(c => c.Name.ToLower() == categoryVM.Name.ToLower() && !c.IsDeleted);
        if (existingCategory != null)
        {
            response.Success = false;
            response.Message = NotificationMessages.AlreadyExisted.Replace("{0}", "Category");
            return response;
        }

        Category category = await _categoryRepository.GetByIdAsync(categoryVM.Id)
                            ?? new Category()
                            {
                                CreatedBy = await _userService.LoggedInUser()
                            };

        category.Name = categoryVM.Name;
        category.Description = categoryVM.Desc;
        category.UpdatedBy = await _userService.LoggedInUser();
        category.UpdatedAt = DateTime.Now;

        if (categoryVM.Id == 0)
        {
            await _categoryRepository.AddAsync(category);
            response.Success = true;
            response.Message = NotificationMessages.Added.Replace("{0}", "Category");
        }
        else
        {
            await _categoryRepository.UpdateAsync(category);
            response.Success = true;
            response.Message = NotificationMessages.Updated.Replace("{0}", "Category");
        }

        return response;
    }
    #endregion Save

    #region Delete
    /*----------------------------------------------------------------Delete Category---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task Delete(long categoryId)
    {
        Category category = await _categoryRepository.GetByIdAsync(categoryId) 
                            ?? throw new NotFoundException(NotificationMessages.NotFound.Replace("{0}","Category"));

        category.IsDeleted = true;
        category.UpdatedBy = await _userService.LoggedInUser();
        category.UpdatedAt = DateTime.Now;

        await _categoryRepository.UpdateAsync(category);
    }

    #endregion
}

using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Common;
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
            orderBy: q => q.OrderBy(c => c.Name)
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
        if (categoryId == 0)
        {
            return new CategoryViewModel();
        }

        Category? category = await _categoryRepository.GetByIdAsync(categoryId);

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
        long createrId = await _userService.LoggedInUser();
        ResponseViewModel response = new();

        Category? category = new();

        if (categoryVM.Id == 0)
        {
            category.CreatedBy = createrId;
        }
        else if (categoryVM.Id > 0)
        {
            category = await _categoryRepository.GetByIdAsync(categoryVM.Id);
        }
        else
        {
            response.Success = false;
            response.Message = NotificationMessages.NotFound.Replace("{0}", "Category");
        }

        category.Name = categoryVM.Name;
        category.Description = categoryVM.Desc;
        category.UpdatedBy = createrId;
        category.UpdatedAt = DateTime.Now;

        if (categoryVM.Id == 0)
        {
            if (await _categoryRepository.AddAsync(category))
            {
                response.Success = true;
                response.Message = NotificationMessages.Added.Replace("{0}", "Category");
            }
            else
            {
                response.Success = false;
                response.Message = NotificationMessages.AddedFailed.Replace("{0}", "Category");
            }
        }
        else
        {
            if (await _categoryRepository.UpdateAsync(category))
            {
                response.Success = true;
                response.Message = NotificationMessages.Updated.Replace("{0}", "Category");
            }
            else
            {
                response.Success = false;
                response.Message = NotificationMessages.UpdatedFailed.Replace("{0}", "Category");
            }
        }

        return response;
    }
    #endregion Save

    #region Delete
    /*----------------------------------------------------------------Delete Category---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<ResponseViewModel> Delete(long categoryId)
    {
        Category? category = await _categoryRepository.GetByIdAsync(categoryId);

        category.IsDeleted = true;
        category.UpdatedBy = await _userService.LoggedInUser();
        category.UpdatedAt = DateTime.Now;

        ResponseViewModel response = new();

        if (await _categoryRepository.UpdateAsync(category))
        {
            response.Success = true;
            response.Message = NotificationMessages.Deleted.Replace("{0}", "Category");
        }
        else
        {
            response.Success = false;
            response.Message = NotificationMessages.DeletedFailed.Replace("{0}", "Category");
        }

        return response;
    }

    #endregion
}

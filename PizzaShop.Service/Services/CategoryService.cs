using System.Threading.Tasks;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Common;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Services;

public class CategoryService : ICategoryService
{
    private readonly IGenericRepository<Category> _categoryRepository;
    private readonly IGenericRepository<User> _userRepository;

    public CategoryService(IGenericRepository<Category> categoryRepository, IGenericRepository<User> userRepository)
    {
        _categoryRepository = categoryRepository;
        _userRepository = userRepository;
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

        Category category = await _categoryRepository.GetByIdAsync(categoryId);

        CategoryViewModel categoryVM = new CategoryViewModel
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
    public async Task<ResponseViewModel> Save(CategoryViewModel categoryVM, string createrEmail)
    {
        User creater = await _userRepository.GetByStringAsync(u => u.Email == createrEmail);
        long createrId = creater.Id;

        Category category = new();

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
            return new ResponseViewModel
            {
                Success = false
            };
        }

        category.Name = categoryVM.Name;
        category.Description = categoryVM.Desc;
        category.UpdatedBy = createrId;
        category.UpdatedAt = DateTime.Now;

        if (categoryVM.Id == 0)
        {
            if (await _categoryRepository.AddAsync(category))
            {
                return new ResponseViewModel
                {
                    Success = true,
                    Message = NotificationMessages.Added.Replace("{0}", "Category")
                };
            }
            else
            {
                return new ResponseViewModel
                {
                    Success = false,
                    Message = NotificationMessages.AddedFailed.Replace("{0}", "Category")
                };
            }
        }
        else if (categoryVM.Id > 0)
        {
            if (await _categoryRepository.UpdateAsync(category))
            {
                return new ResponseViewModel
                {
                    Success = true,
                    Message = NotificationMessages.Updated.Replace("{0}", "Category")
                };
            }
            else
            {
                return new ResponseViewModel
                {
                    Success = false,
                    Message = NotificationMessages.UpdatedFailed.Replace("{0}", "Category")
                };
            }
        }
        else
        {
            return new ResponseViewModel
            {
                Success = false,
                Message = NotificationMessages.TryAgain
            }; ;
        }
    }
    #endregion Save

    #region Delete
    /*----------------------------------------------------------------Delete Category---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<bool> Delete(long categoryId, string createrEmail)
    {
        User? creater = await _userRepository.GetByStringAsync(u => u.Email == createrEmail);
        Category? category = await _categoryRepository.GetByIdAsync(categoryId);

        category.IsDeleted = true;
        category.UpdatedBy = creater.Id;
        category.UpdatedAt = DateTime.Now;

        return await _categoryRepository.UpdateAsync(category);
    }

    #endregion
}

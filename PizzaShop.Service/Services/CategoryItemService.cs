using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Helpers;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Services;

public class CategoryItemService : ICategoryItemService
{
    private readonly IGenericRepository<Category> _categoryRepository;
    private readonly IGenericRepository<User> _userRepository;
    private readonly IGenericRepository<Item> _itemRepository;

    public CategoryItemService(IGenericRepository<Category> categoryRepository, IGenericRepository<User> userRepository, IGenericRepository<Item> itemRepository)
    {
        _categoryRepository = categoryRepository;
        _userRepository = userRepository;
        _itemRepository = itemRepository;
    }


    #region Display Category
    /*-----------------------------------------------------------Display Category---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/

    public List<CategoryViewModel> GetCategory()
    {
        var categories = _categoryRepository.GetByCondition(c => c.IsDeleted == false)
        .Select(category => new CategoryViewModel
        {
            CategoryId = category.Id,
            CategoryName = category.Name,
            CategoryDesc = category.Description
        })
        .ToList();

        return categories;
    }
#endregion  


#region Edit Category
/*--------------------------------------------------------------Get Category for Editing---------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public CategoryViewModel GetCategoryById(long categoryId)
    {
        var category = _categoryRepository.GetByIdAsync(categoryId);

        CategoryViewModel categoryVM = new CategoryViewModel
        {
            CategoryId = category.Result.Id,
            CategoryName = category.Result.Name,
            CategoryDesc = category.Result.Description
        };

        return categoryVM;
    }

/*--------------------------------------------------------Update the edited Category---------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<bool> EditCategory(CategoryViewModel model)
    {
        try
        {
            Category category = await _categoryRepository.GetByIdAsync(model.CategoryId);

            category.Name = model.CategoryName;
            category.Description = model.CategoryDesc;

            await _categoryRepository.UpdateAsync(category);
            return true;
        }
        catch(Exception ex)
        {
            return false;
        }
    }
#endregion


#region  Add Category
/*-------------------------------------------------------------Add Category---------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<bool> AddCategory(CategoryViewModel model,string createrEmail)
    {
        var creater = await _userRepository.GetByStringAsync(u => u.Email == createrEmail);
        
            Category category = new Category
            {
                Name = model.CategoryName,
                Description = model.CategoryDesc,
                CreatedBy = creater.Id
                
            };
        try
        {
            await _categoryRepository.AddAsync(category);
            return true;
        }
        catch(Exception)
        {
            return false;
        }
        
    }
#endregion

#region Delete Category
/*----------------------------------------------------------------Soft Delete Category---------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<bool> SoftDelete(long categoryId)
    {
        Category category = await _categoryRepository.GetByIdAsync(categoryId);

        category.IsDeleted = true;

        try{
            await _categoryRepository.UpdateAsync(category);
            return true;
        }
        catch(Exception)
        {
            return false;
        }
    }

#endregion

#region Display Items
/*-----------------------------------------------------------------Display Items---------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public ItemsPaginationViewModel GetItems(int pageSize, int pageNumber)
    {
        ItemsPaginationViewModel model = new(){ Page = new() };

        var itemsDB = _itemRepository.GetPagedRecords(pageSize, pageNumber, orderBy: q=> q.OrderBy(u => u.Id));
        model.Items = itemsDB.records.Select(item => new ItemInfoViewModel()
        {
            ItemId = item.Id,
            ItemName = item.Name,
            ItemType = "Veg",
            Rate = item.Rate,
            Quantity = item.Quantity,
            Available = item.Available
        }).ToList();

        model.Page.SetPagination(itemsDB.totalRecord, pageSize, pageNumber);
        return model;
    }

#endregion
}

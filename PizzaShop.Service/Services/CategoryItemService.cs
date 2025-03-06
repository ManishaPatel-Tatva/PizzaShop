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

#region Category

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

#endregion Category

#region Items

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

#endregion Display Items

#region Edit Item

    public async Task<AddItemViewModel> GetEditItem(long itemId)
    {
    
        Item item = await _itemRepository.GetByIdAsync(itemId);
        if (item == null)
            return null;

        AddItemViewModel model = new AddItemViewModel
        {
            CategoryId = item.CategoryId,
            Name = item.Name,
            ItemTypeId = item.FoodTypeId,
            Rate = item.Rate,
            Quantity = item.Quantity,
            UnitId = item.UnitId,
            Available = item.Available,
            DefaultTax = item.DefaultTax,
            TaxPercentage = item.Tax,
            ShortCode = item.ShortCode,
            Description = item.Description,
            ItemImageUrl = item.ImageUrl,
        };

        return model;
    }

    public async Task<bool> EditItem(AddItemViewModel model)
    {
        try
        {
            Item item = await _itemRepository.GetByIdAsync(model.ItemId);

            item.CategoryId = model.CategoryId;
            item.Name = model.Name;
            item.FoodTypeId = model.ItemTypeId;
            item.Rate = model.Rate;
            item.Quantity = model.Quantity;
            item.UnitId = model.UnitId;
            item.Available = model.Available;
            item.DefaultTax = model.DefaultTax;
            item.Tax = model.TaxPercentage;
            item.ShortCode = model.ShortCode;
            item.Description = model.Description;
            item.ImageUrl = model.ItemImageUrl;

             // Handle Image Upload
            if (model.Image != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/itemImages");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string fileName = $"{Guid.NewGuid()}_{model.Image.FileName}";
                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Image.CopyToAsync(stream);
                }

                item.ImageUrl = $"/itemImages/{fileName}";
            }

            await _itemRepository.UpdateAsync(item);
            return true;
        }
        catch(Exception)
        {
            return false;
        }
    }

#endregion Edit Item

#region  Add Item

     public async Task<bool> AddItem(AddItemViewModel model,string createrEmail)
    {
        var creater = await _userRepository.GetByStringAsync(u => u.Email == createrEmail);
        try
        {
            Item item = new Item
            {
                CategoryId = model.CategoryId,
                Name = model.Name,
                FoodTypeId = model.ItemTypeId,
                Rate = model.Rate,
                Quantity = model.Quantity,
                UnitId = model.UnitId,
                Available = model.Available,
                DefaultTax = model.DefaultTax,
                Tax = model.TaxPercentage,
                ShortCode = model.ShortCode,
                Description = model.Description,
                ImageUrl = model.ItemImageUrl,
                CreatedBy = creater.Id
            };

             // Handle Image Upload
            if (model.Image != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/itemImages");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string fileName = $"{Guid.NewGuid()}_{model.Image.FileName}";
                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Image.CopyToAsync(stream);
                }

                item.ImageUrl = $"/itemImages/{fileName}";
            }

            await _itemRepository.AddAsync(item);
            return true;
        }
        catch(Exception)
        {
            return false;
        }
    }   


#endregion Add Item

#endregion Items
}


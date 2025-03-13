using System.Linq;
using System.Linq.Expressions;
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
    private readonly IGenericRepository<FoodType> _foodTypeRepository;
    private readonly IGenericRepository<Unit> _unitRepository;
    private readonly IGenericRepository<ModifierGroup> _modifierGroupRepository;
    private readonly IGenericRepository<ItemModifierGroup> _itemModifierGroupRepository;
    private readonly IGenericRepository<Modifier> _modifierRepository;

    public CategoryItemService(IGenericRepository<Category> categoryRepository, IGenericRepository<User> userRepository, IGenericRepository<Item> itemRepository, IGenericRepository<FoodType> foodTypeRepository, IGenericRepository<Unit> unitRepository, IGenericRepository<ModifierGroup> modifierGroupRepository, IGenericRepository<ItemModifierGroup> itemModifierGroupRepository, IGenericRepository<Modifier> modifierRepository)
    {
        _categoryRepository = categoryRepository;
        _userRepository = userRepository;
        _itemRepository = itemRepository;
        _foodTypeRepository = foodTypeRepository;
        _unitRepository = unitRepository;
        _modifierGroupRepository = modifierGroupRepository;
        _itemModifierGroupRepository = itemModifierGroupRepository;
        _modifierRepository = modifierRepository;
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
    public async Task<bool> AddCategory(CategoryViewModel model, string createrEmail)
    {
        var creater = await _userRepository.GetByStringAsync(u => u.Email == createrEmail);

        Category category = new Category
        {
            Name = model.CategoryName,
            Description = model.CategoryDesc,
            CreatedBy = creater.Id
        };
        return await _categoryRepository.AddAsync(category);
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
        Category category = await _categoryRepository.GetByIdAsync(model.CategoryId);

        category.Name = model.CategoryName;
        category.Description = model.CategoryDesc;

        return await _categoryRepository.UpdateAsync(category);
    }
    #endregion

    #region Delete Category
    /*----------------------------------------------------------------Soft Delete Category---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<bool> SoftDelete(long categoryId)
    {
        Category category = await _categoryRepository.GetByIdAsync(categoryId);

        category.IsDeleted = true;

        return await _categoryRepository.UpdateAsync(category);
    }

    #endregion

    #endregion Category

    #region Items

    #region Display Items
    /*-----------------------------------------------------------------Display Items---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/

    public async Task<ItemsPaginationViewModel> GetPagedItems(long categoryId, int pageSize, int pageNumber, string search)
    {
        var (items, totalRecord) = await _itemRepository.GetPagedRecordsAsync(
            pageSize,
            pageNumber,
            filter: i => !i.IsDeleted &&
                    i.CategoryId == categoryId &&
                    (string.IsNullOrEmpty(search.ToLower()) ||
                    i.Name.ToLower() == search.ToLower()),
            orderBy: q => q.OrderBy(u => u.Id),
            includes: new List<Expression<Func<Item, object>>> { u => u.FoodType }
        );

        ItemsPaginationViewModel model = new() { Page = new() };

        model.Items = items.Select(i => new ItemInfoViewModel()
        {
            ItemId = i.Id,
            ItemImageUrl = i.ImageUrl,
            ItemName = i.Name,
            ItemType = i.FoodType.ImageUrl,
            Rate = i.Rate,
            Quantity = i.Quantity,
            Available = i.Available
        }).ToList();

        model.Page.SetPagination(totalRecord, pageSize, pageNumber);
        return model;
    }

    #endregion Display Items

    #region Get Add/Edit Item
    public async Task<AddItemViewModel> GetEditItem(long itemId)
    {
        AddItemViewModel model = new()
        {
            Name = "",
            Categories = _categoryRepository.GetAll().ToList(),
            ItemTypes = _foodTypeRepository.GetAll().ToList(),
            Units = _unitRepository.GetAll().ToList(),
            ModifierGroups = _modifierGroupRepository.GetAll().ToList()
        };

        if (itemId == 0)
        {
            return model;
        }

        Item item = await _itemRepository.GetByIdAsync(itemId);
        if (item == null)
            return model;

        model.CategoryId = item.CategoryId;
        model.Name = item.Name;
        model.ItemTypeId = item.FoodTypeId;
        model.Rate = item.Rate;
        model.Quantity = item.Quantity;
        model.UnitId = item.UnitId;
        model.Available = item.Available;
        model.DefaultTax = item.DefaultTax;
        model.TaxPercentage = item.Tax;
        model.ShortCode = item.ShortCode;
        model.Description = item.Description;
        model.ItemImageUrl = item.ImageUrl;

        return model;
    }

    public async Task<ItemModifierViewModel> GetModifierOnSelection(long modifierGroupId)
    {
        ItemModifierGroup itemModifierGroup = await _itemModifierGroupRepository.GetByStringAsync(i => i.ModifierGroupId == modifierGroupId);     //needs to include ModifierGroup table
        ModifierGroup modifierGroup = await _modifierGroupRepository.GetByIdAsync(modifierGroupId);

        if (modifierGroup == null)
        {

        }
        
        List<ModifierViewModel> modifierList = _modifierRepository.GetByCondition(m => m.ModifierGroupId == modifierGroupId)
        .Select(m => new ModifierViewModel()
        {
            ModifierId = m.Id,
            ModifierName = m.Name,
            Rate = m.Rate,
        }).ToList();

        ItemModifierViewModel model = new ItemModifierViewModel
        {
            ModifierGroupId = modifierGroupId,
            ModifierGroupName = modifierGroup.Name,
            MinAllowed = modifierList.Count(),
            MaxAllowed = itemModifierGroup.MaxAllowed,  
            ModifierList = modifierList                                       
        };

        return model;
    }


    #endregion Get Add/Edit Item 

    #region  Add Item
    public async Task<bool> AddItem(AddItemViewModel model, string createrEmail)
    {
        var creater = await _userRepository.GetByStringAsync(u => u.Email == createrEmail);

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
        return await _itemRepository.AddAsync(item);
    }
    #endregion Add Item

    #region Update Item
    public async Task<bool> UpdateItem(AddItemViewModel model)
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

        return await _itemRepository.UpdateAsync(item); ;
    }

    #endregion Update Item

    #region Soft Delete

    public async Task<bool> SoftDeleteItem(long id)
    {
        Item item = await _itemRepository.GetByIdAsync(id);

        if (item == null)
            return false;

        item.IsDeleted = true;
        return await _itemRepository.UpdateAsync(item);
    }

    public async Task<bool> MassDeleteItems(List<long> itemsList)
    {
        bool isDeleted;
        foreach (long id in itemsList)
        {
            Item item = await _itemRepository.GetByIdAsync(id);

            if (item == null)
                return false;

            item.IsDeleted = true;
            isDeleted = await _itemRepository.UpdateAsync(item);
            if (!isDeleted)
                return false;
        }
        return true;
    }

    #endregion Soft Delete

    #endregion Items
}


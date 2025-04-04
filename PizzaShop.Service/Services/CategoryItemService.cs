using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
    

    public CategoryItemService(IGenericRepository<Category> categoryRepository, IGenericRepository<User> userRepository, IGenericRepository<Item> itemRepository, IGenericRepository<FoodType> foodTypeRepository, IGenericRepository<Unit> unitRepository, IGenericRepository<ModifierGroup> modifierGroupRepository, IGenericRepository<ItemModifierGroup> itemModifierGroupRepository)
    {
        _categoryRepository = categoryRepository;
        _userRepository = userRepository;
        _itemRepository = itemRepository;
        _foodTypeRepository = foodTypeRepository;
        _unitRepository = unitRepository;
        _modifierGroupRepository = modifierGroupRepository;
        _itemModifierGroupRepository = itemModifierGroupRepository;
    }

    #region Category

    #region Display Category
    /*-----------------------------------------------------------Display Category---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public List<CategoryViewModel> GetCategory()
    {
        List<CategoryViewModel>? categories = _categoryRepository.GetByCondition(c => c.IsDeleted == false).Result
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
    /*-------------------------------------------------------------Save Category---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<bool> SaveCategory(CategoryViewModel model, string createrEmail)
    {
        User creater = await _userRepository.GetByStringAsync(u => u.Email == createrEmail);
        long createrId = creater.Id;

        if(createrId == 0){
            return false;
        }

        if(model.CategoryId == 0)
        {
            return await AddCategory( model,  createrId);
        }
        else if(model.CategoryId > 0)
        {
            return await EditCategory(model, createrId);
        }
        else{
            return false;
        }
    }


    /*-------------------------------------------------------------Add Category---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<bool> AddCategory(CategoryViewModel model, long createrId)
    {

        Category category = new Category
        {
            Name = model.CategoryName,
            Description = model.CategoryDesc,
            CreatedBy = createrId
        };
        return await _categoryRepository.AddAsync(category);
    }
    #endregion

    #region Edit Category
    /*--------------------------------------------------------------Get Category for Editing---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<CategoryViewModel> GetCategoryById(long categoryId)
    {
        if(categoryId == 0)
        {
            return new CategoryViewModel();
        }

        Category category = await _categoryRepository.GetByIdAsync(categoryId);

        CategoryViewModel categoryVM = new CategoryViewModel
        {
            CategoryId = category.Id,
            CategoryName = category.Name,
            CategoryDesc = category.Description
        };

        return categoryVM;
    }

    /*--------------------------------------------------------Update the edited Category---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<bool> EditCategory(CategoryViewModel model, long createrId)
    {
        Category category = await _categoryRepository.GetByIdAsync(model.CategoryId);

        category.Name = model.CategoryName;
        category.Description = model.CategoryDesc;
        category.UpdatedBy = createrId;
        category.UpdatedAt = DateTime.Now;

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
        (IEnumerable<Item> items, int totalRecord) = await _itemRepository.GetPagedRecordsAsync(
            pageSize,
            pageNumber,
            predicate: i => !i.IsDeleted &&
                    i.CategoryId == categoryId &&
                    (string.IsNullOrEmpty(search.ToLower()) ||
                    i.Name.ToLower().Contains(search.ToLower())),
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
    /*-----------------------------------------------------------Get Add/Update Item---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
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

        model.ItemId = item.Id;
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

        model.ItemModifierGroups = _itemModifierGroupRepository.GetByCondition(
            i => i.ItemId == itemId && !i.IsDeleted,
            includes: new List<Expression<Func<ItemModifierGroup, object>>>
            {
                img => img.ModifierGroup
            },
            thenIncludes: new List<Func<IQueryable<ItemModifierGroup>, IQueryable<ItemModifierGroup>>>
            {
                q => q.Include(img => img.ModifierGroup)
                    .ThenInclude(mg => mg.ModifierMappings)
                    .ThenInclude(m => m.Modifier) // Deepest level include
            }
            )
            .Result
            .Select(i => new ItemModifierViewModel
            {
                ModifierGroupId = i.ModifierGroupId,
                ModifierGroupName = i.ModifierGroup.Name,
                MinAllowed = i.MinAllowed,
                MaxAllowed = i.MaxAllowed,
                ModifierList = i.ModifierGroup.ModifierMappings
                .Where(i => !i.IsDeleted)
                .Select(m => new ModifierViewModel
                {
                    ModifierId = m.Modifier.Id,
                    ModifierName = m.Modifier.Name,
                    Rate = m.Modifier.Rate
                }).ToList()
            }).ToList();

        return model;
    }



    /*-----------------------------------------------------------Get Modifier on Selection---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<ItemModifierViewModel> GetModifierOnSelection(long modifierGroupId)
    {
        ItemModifierViewModel itemModifierGroups =  _modifierGroupRepository.GetByCondition(
            m => m.Id == modifierGroupId && !m.IsDeleted,
            includes: new List<Expression<Func<ModifierGroup, object>>>
            {
                mg => mg.ModifierMappings
            },
            thenIncludes: new List<Func<IQueryable<ModifierGroup>, IQueryable<ModifierGroup>>>
            {
                q => q.Include(mg => mg.ModifierMappings)
                    .ThenInclude(m => m.Modifier) // Deepest level include
            }
            )
            .Result
            .Select(mg => new ItemModifierViewModel
            {
                ModifierGroupId = mg.Id,
                ModifierGroupName = mg.Name,
                ModifierList = mg.ModifierMappings
                .Where(i => !i.IsDeleted)
                .Select(mm => new ModifierViewModel
                {
                    ModifierId = mm.Modifier.Id,
                    ModifierName = mm.Modifier.Name,
                    Rate = mm.Modifier.Rate
                }).ToList()
            }).First();

        return itemModifierGroups;
    }


    #endregion Get Add/Edit Item 

    #region  Add Item

    public async Task<bool> AddUpdateItem(AddItemViewModel model, string createrEmail)
    {
        User creater = await _userRepository.GetByStringAsync(u => u.Email == createrEmail);
        long createrId = creater.Id;

        if (model.ItemId == 0)
        {
            return await AddItem(model, createrId);
        }
        else if (model.ItemId > 0)
        {
            return await UpdateItem(model, createrId);
        }
        else
        {
            return false;
        }

    }


    public async Task<bool> AddItem(AddItemViewModel model, long createrId)
    {

        Item item = new()
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
            CreatedBy = createrId,
            ImageUrl = model.ItemImageUrl,
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

        long itemId = await _itemRepository.AddAsyncReturnId(item);

        if (itemId < 1)
        {
            return false;
        }

        if (itemId > 0)
        {
            foreach (ItemModifierViewModel modifierGroup in model.ItemModifierGroups)
            {
                bool success = await AddItemModifierGroup(itemId, modifierGroup, createrId);
                if (!success)
                    return false;
            }
        }
        return true;

    }

    public async Task<bool> AddItemModifierGroup(long itemId, ItemModifierViewModel model, long createrId)
    {
        ItemModifierGroup itemModifierGroup = new()
        {
            ItemId = itemId,
            ModifierGroupId = model.ModifierGroupId,
            MinAllowed = model.MinAllowed,
            MaxAllowed = model.MaxAllowed,
            CreatedBy = createrId
        };
        return await _itemModifierGroupRepository.AddAsync(itemModifierGroup);
    }


    #endregion Add Item

    #region Update Item
    public async Task<bool> UpdateItem(AddItemViewModel model, long createrId)
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

        bool itemUpdated = await _itemRepository.UpdateAsync(item);

        if (!itemUpdated)
            return false;

        return await UpdateItemModifierGroup(model.ItemId, model.ItemModifierGroups, createrId);
       
    }

    public async Task<bool> UpdateItemModifierGroup(long itemId, List<ItemModifierViewModel> itemModifierList, long createrId)
    {
        List<long> existingGroupList = _itemModifierGroupRepository
        .GetByCondition(m => m.ItemId == itemId && !m.IsDeleted)
        .Result
        .Select(mg => mg.ModifierGroupId)
        .ToList();

        List<long> currentGroupList = itemModifierList.Select(mg => mg.ModifierGroupId).ToList();

        List<long> removeGroupList = existingGroupList.Except(currentGroupList).ToList();

        foreach (long groupId in removeGroupList)
        {
            ItemModifierGroup? itemModifierGroup = await _itemModifierGroupRepository.GetByStringAsync(mg => mg.ModifierGroupId == groupId && mg.ItemId == itemId && !mg.IsDeleted);
            itemModifierGroup.IsDeleted = true;
            bool success = await _itemModifierGroupRepository.UpdateAsync(itemModifierGroup);
            if (!success)
                return false;
        }

        foreach (ItemModifierViewModel itemModifier in itemModifierList)
        {
            ItemModifierGroup existingGroup = await _itemModifierGroupRepository.GetByStringAsync(mg => mg.ItemId == itemModifier.ItemId && mg.ModifierGroupId == itemModifier.ModifierGroupId && mg.IsDeleted == false);
            if (existingGroup == null)
            {
                bool success = await AddItemModifierGroup(itemId, itemModifier, createrId);
                if (!success)
                    return false;
            }
            else
            {
                existingGroup.MinAllowed = itemModifier.MinAllowed;
                existingGroup.MaxAllowed = itemModifier.MaxAllowed;
                bool success = await _itemModifierGroupRepository.UpdateAsync(existingGroup);
                if (!success)
                    return false;

            }
        }

        return true;
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


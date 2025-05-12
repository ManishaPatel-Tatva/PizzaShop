using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Common;
using PizzaShop.Service.Exceptions;
using PizzaShop.Service.Helpers;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Services;
public class ItemService : IItemService
{
    private readonly IGenericRepository<Category> _categoryRepository;
    private readonly IGenericRepository<Item> _itemRepository;
    private readonly IGenericRepository<FoodType> _foodTypeRepository;
    private readonly IGenericRepository<Unit> _unitRepository;
    private readonly IGenericRepository<ModifierGroup> _modifierGroupRepository;
    private readonly IGenericRepository<ItemModifierGroup> _itemModifierGroupRepository;
    private readonly IUserService _userService;
    private readonly IItemModifierService _itemModifierService;

    public ItemService(IGenericRepository<Category> categoryRepository, IGenericRepository<Item> itemRepository, IGenericRepository<FoodType> foodTypeRepository, IGenericRepository<Unit> unitRepository, IGenericRepository<ModifierGroup> modifierGroupRepository, IGenericRepository<ItemModifierGroup> itemModifierGroupRepository, IUserService userService, IItemModifierService itemModifierService)
    {
        _categoryRepository = categoryRepository;
        _itemRepository = itemRepository;
        _foodTypeRepository = foodTypeRepository;
        _unitRepository = unitRepository;
        _modifierGroupRepository = modifierGroupRepository;
        _itemModifierGroupRepository = itemModifierGroupRepository;
        _userService = userService;
        _itemModifierService = itemModifierService;
    }

    #region Get
    /*----------------------------------------------------------------- Items Pagination---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<ItemsPaginationViewModel> Get(long categoryId, FilterViewModel filter)
    {
        IEnumerable<Item> items = await _itemRepository.GetByCondition(
            predicate: i => !i.IsDeleted &&
                    i.CategoryId == categoryId &&
                    (string.IsNullOrEmpty(filter.Search.ToLower()) ||
                    i.Name.ToLower().Contains(filter.Search.ToLower())),
            orderBy: q => q.OrderBy(u => u.Id),
            includes: new List<Expression<Func<Item, object>>> { u => u.FoodType }
        );

        (items, int totalRecord) = _itemRepository.GetPagedRecords(filter.PageSize, filter.PageNumber, items);

        ItemsPaginationViewModel model = new()
        {
            Page = new(),
            Items = items.Select(i => new ItemInfoViewModel()
            {
                Id = i.Id,
                ImageUrl = i.ImageUrl,
                Name = i.Name,
                Type = i.FoodType.ImageUrl,
                Rate = i.Rate,
                Quantity = i.Quantity,
                Available = i.Available
            }).ToList()
        };

        model.Page.SetPagination(totalRecord, filter.PageSize, filter.PageNumber);
        return model;
    }

    /*-----------------------------------------------------------Get Item---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<ItemViewModel> Get(long itemId)
    {
        ItemViewModel itemVM = new()
        {
            Name = "",
            Categories = _categoryRepository.GetAll().ToList(),
            FoodType = _foodTypeRepository.GetAll().ToList(),
            Units = _unitRepository.GetAll().ToList(),
            ModifierGroups = _modifierGroupRepository.GetAll().ToList()
        };

        Item? item = await _itemRepository.GetByIdAsync(itemId);
        if (item == null)
        {
            return itemVM;
        }

        itemVM.Id = item.Id;
        itemVM.CategoryId = item.CategoryId;
        itemVM.Name = item.Name;
        itemVM.ItemTypeId = item.FoodTypeId;
        itemVM.Rate = item.Rate;
        itemVM.Quantity = item.Quantity;
        itemVM.UnitId = item.UnitId;
        itemVM.Available = item.Available;
        itemVM.DefaultTax = item.DefaultTax;
        itemVM.TaxPercentage = item.Tax;
        itemVM.ShortCode = item.ShortCode;
        itemVM.Description = item.Description;
        itemVM.ImageUrl = item.ImageUrl;

        itemVM.ItemModifierGroups = await _itemModifierService.List(itemId);

        return itemVM;
    }


    #endregion Get 

    #region  Save

    public async Task<ResponseViewModel> Save(ItemViewModel itemVM)
    {
        Item item = await _itemRepository.GetByIdAsync(itemVM.Id) ?? new Item();

        ResponseViewModel response = new();

        if (item.Id == 0)
        {
            item.CreatedBy = await _userService.LoggedInUser();
        }

        item.CategoryId = itemVM.CategoryId;
        item.Name = itemVM.Name;
        item.FoodTypeId = itemVM.ItemTypeId;
        item.Rate = itemVM.Rate;
        item.Quantity = itemVM.Quantity;
        item.UnitId = itemVM.UnitId;
        item.Available = itemVM.Available;
        item.DefaultTax = itemVM.DefaultTax;
        item.Tax = itemVM.TaxPercentage;
        item.ShortCode = itemVM.ShortCode;
        item.Description = itemVM.Description;

        item.UpdatedAt = DateTime.Now;
        item.UpdatedBy = await _userService.LoggedInUser();

        // Handle Image Upload
        if (itemVM.Image != null)
        {
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/itemImages");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string fileName = $"{Guid.NewGuid()}_{itemVM.Image.FileName}";
            string filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await itemVM.Image.CopyToAsync(stream);
            }

            item.ImageUrl = $"/itemImages/{fileName}";
        }

        if (itemVM.Id == 0)
        {
            item.Id = await _itemRepository.AddAsyncReturnId(item);
        }
        else
        {
            await _itemRepository.UpdateAsync(item);
        }

        await _itemModifierService.Save(itemVM.Id, itemVM.ItemModifierGroups);

        response.Success = true;
        response.Message = itemVM.Id == 0 ? NotificationMessages.Added.Replace("{0}", "Item") : NotificationMessages.Updated.Replace("{0}", "Item");

        return response;
    }

    #endregion Save

    #region Delete

    public async Task Delete(long id)
    {
        Item item = await _itemRepository.GetByIdAsync(id) ?? throw new NotFoundException(NotificationMessages.NotFound.Replace("{0}", "Item"));

        item.IsDeleted = true;
        item.UpdatedAt = DateTime.Now;
        item.UpdatedBy = await _userService.LoggedInUser();

        await _itemRepository.UpdateAsync(item);
    }

    public async Task Delete(List<long> items)
    {
        foreach (long id in items)
        {
            await Delete(id);
        }
    }
    #endregion Delete

}



using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Common;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Services;

public class AppMenuService : IAppMenuService
{
    private readonly IGenericRepository<Item> _itemRepository;
    private readonly IGenericRepository<OrderTableMapping> _orderTableRepository;
    private readonly ICategoryService _categoryService;
    private readonly IItemService _itemService;
    private readonly IOrderService _orderService;

    public AppMenuService(IGenericRepository<Item> itemRepository, IGenericRepository<OrderTableMapping> orderTableRepository, ICategoryService categoryService, IItemService itemService, IOrderService orderService)
    {
        _itemRepository = itemRepository;
        _orderTableRepository = orderTableRepository;
        _categoryService = categoryService;
        _itemService = itemService;
        _orderService = orderService;

    }

    public async Task<AppMenuViewModel> Get(long customerId)
    {
        AppMenuViewModel appMenu = new()
        {
            Categories = await _categoryService.Get(),
            CustomerId = customerId
        };

        if (customerId == 0)
        {
            return appMenu;
        }
        else
        {
            IEnumerable<OrderTableMapping>? mapping = await _orderTableRepository.GetByCondition(
                predicate: otm => !otm.IsDeleted,
                includes: new List<Expression<Func<OrderTableMapping, object>>>
                {
                    otm => otm.Order
                },
                thenIncludes: new List<Func<IQueryable<OrderTableMapping>, IQueryable<OrderTableMapping>>>
                {
                    q => q.Include(otm => otm.Table)
                        .ThenInclude(t => t.Section).Where(otm => otm.CustomerId == customerId)
                }
            );

            appMenu.SectionName = mapping.Select(m => m.Table.Section.Name).FirstOrDefault()!;
            appMenu.Tables = mapping.Select(m => m.Table.Name).ToList();

            long? orderId = mapping.Select(m => m.OrderId).FirstOrDefault();
            if (orderId == null)
            {
                return appMenu;
            }
            else
            {
                appMenu.Order = await _orderService.Get((long)orderId);
                return appMenu;
            }

        }

    }

    public async Task<List<ItemInfoViewModel>> List(long categoryId, string search)
    {
        IEnumerable<Item>? list = await _itemRepository.GetByCondition(
            predicate: i => !i.IsDeleted &&
                        (categoryId == 0
                        || categoryId == -1 && i.IsFavourite
                        || i.CategoryId == categoryId) &&
                        (string.IsNullOrEmpty(search) ||
                        i.Name.ToLower().Contains(search.ToLower())),
            orderBy: q => q.OrderBy(i => i.Id),
            includes: new List<Expression<Func<Item, object>>>
            {
                i => i.FoodType
            }
        );

        List<ItemInfoViewModel> items = list.Select(i => new ItemInfoViewModel
        {
            Id = i.Id,
            ImageUrl = i.ImageUrl,
            Name = i.Name,
            Type = i.FoodType.Name,
            Rate = i.Rate,
            IsFavourite = i.IsFavourite
        }).ToList();

        return items;
    }

    public async Task<ResponseViewModel> FavouriteItem(long itemId)
    {
        Item? item = await _itemRepository.GetByIdAsync(itemId);

        ResponseViewModel response = new();

        if (item == null)
        {
            response.Success = false;
            response.Message = NotificationMessages.NotFound.Replace("{0}", "Item");
            return response;
        }

        item.IsFavourite = !item.IsFavourite;
        response.Success = await _itemRepository.UpdateAsync(item);
        response.Message = response.Success ? NotificationMessages.Updated.Replace("{0}", "Item") : NotificationMessages.UpdatedFailed.Replace("{0}", "Item");
        return response;
    }

}

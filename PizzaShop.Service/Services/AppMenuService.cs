
using System.Linq.Expressions;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Common;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Services;

public class AppMenuService : IAppMenuService
{
    private readonly IGenericRepository<Item> _itemRepository;

    public AppMenuService(IGenericRepository<Item> itemRepository)
    {
        _itemRepository = itemRepository;
    }

    public async Task<List<ItemInfoViewModel>> Get(long categoryId, string search)
    {
        IEnumerable<Item>? list = await _itemRepository.GetByCondition(
            predicate: i => !i.IsDeleted &&
                        ( categoryId == 0 
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
        Item? item= await _itemRepository.GetByIdAsync(itemId);

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

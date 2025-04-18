using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface ICategoryItemService
{

    Task<ItemsPaginationViewModel> GetPagedItems(long categoryId, int pageSize, int pageNumber, string search);

    Task<AddItemViewModel> GetEditItem(long itemId);
    Task<ItemModifierViewModel> GetModifierOnSelection(long modifierGroupId);

    Task<bool> AddUpdateItem(AddItemViewModel model, string createrEmail);
    
    Task<bool> UpdateItem(AddItemViewModel model, long createrId);
    Task<bool> UpdateItemModifierGroup(long itemId, List<ItemModifierViewModel> itemModifierList, long createrId);
    Task<bool> AddItemModifierGroup(long itemId, ItemModifierViewModel model, long createrId);

    Task<bool> AddItem(AddItemViewModel model,long createrId);

    Task<bool> SoftDeleteItem(long id);

    Task<bool> MassDeleteItems(List<long> itemsList);


}

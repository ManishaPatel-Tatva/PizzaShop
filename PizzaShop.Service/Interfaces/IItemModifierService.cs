using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IItemModifierService
{
    Task<ItemModifierViewModel> Get(long modifierGroupId);
    Task<List<ItemModifierViewModel>> List(long itemId);
    Task Save(ItemModifierViewModel itemModifierVM);
    Task Save(long itemId, List<ItemModifierViewModel> itemModifierList);
    Task Delete(long itemId, long modifierGroupId);
}

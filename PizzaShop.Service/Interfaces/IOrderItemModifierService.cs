using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IOrderItemModifierService
{
    Task<bool> Add(List<ModifierViewModel> modifiers, long orderItemId);
}

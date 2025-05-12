using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IModifierGroupService
{
    List<ModifierGroupViewModel> Get();
    Task<ModifierGroupViewModel> Get(long modifierGroupId);
    Task<ResponseViewModel> Save(ModifierGroupViewModel modifierGroupVM);
    Task Delete(long mgId);
}

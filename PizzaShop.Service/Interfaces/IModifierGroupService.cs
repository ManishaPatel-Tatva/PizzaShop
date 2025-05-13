using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IModifierGroupService
{
    Task<List<ModifierGroupViewModel>> Get();
    Task<ModifierGroupViewModel> Get(long modifierGroupId);
    Task<ResponseViewModel> Save(ModifierGroupViewModel modifierGroupVM);
    Task Delete(long mgId);
}

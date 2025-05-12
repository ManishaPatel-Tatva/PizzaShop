using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IModifierMappingService
{
    Task Add(long modifierGroupId, long modifierId);
    Task UpdateModifierGroupMapping(long modifierGroupId, List<long> modifierList);
    Task UpdateModifierMapping(long modifierId, List<long> modifierGroupList);
    Task Delete(long mgId);
    Task Delete(long mgId, long modifierId);
}

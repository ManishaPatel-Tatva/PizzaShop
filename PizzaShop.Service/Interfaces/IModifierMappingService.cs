using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IModifierMappingService
{
    Task<bool> Add(long modifierGroupId, long modifierId);
    Task<bool> UpdateModifierGroupMapping(long modifierGroupId, List<long> modifierList);
    Task<bool> UpdateModifierMapping(long modifierId, List<long> modifierGroupList);
    Task<ResponseViewModel> Delete(long mgId);
    Task<bool> Delete(long mgId, long modifierId);
}

using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IModifierService
{
    List<ModifierGroupViewModel> GetModifierGroups();
    Task<ModifierGroupViewModel> GetModifierGroup(long modifierGroupId);
    Task<bool> SaveModifierGroup(ModifierGroupViewModel model, string createrEmail);
    Task<bool> AddModifierGroup(ModifierGroupViewModel model, long createrId);
    Task<bool> UpdateModifierGroup(ModifierGroupViewModel model, long createrId);
    Task<bool> DeleteModifierGroup(long modifierGroupId);
    Task<ModifiersPaginationViewModel> GetPagedModifiers(long modifierGroupId, int pageSize, int pageNumber, string search);
    Task<ModifiersPaginationViewModel> GetAllModifiers(int pageSize, int pageNumber, string search);
    Task<ModifierViewModel> GetModifier(long modifierId);
    Task<bool> SaveModifier(ModifierViewModel model, string createrEmail);
    Task<bool> AddModifier(ModifierViewModel model, long createrId);
    Task<bool> UpdateModifier(ModifierViewModel model, long createrId);
    Task<bool> DeleteModifier(long modifierId);
    Task<bool> MassDeleteModifiers(List<long> modifierIdList);
}

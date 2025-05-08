using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Common;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Services;

public class ModifierMappingService : IModifierMappingService
{
    private readonly IGenericRepository<ModifierMapping> _modifierMappingRepository;
    private readonly IUserService _userService;

    public ModifierMappingService(IGenericRepository<ModifierMapping> modifierMappingRepository, IUserService userService)
    {
        _modifierMappingRepository = modifierMappingRepository;
        _userService = userService;

    }

    public async Task<bool> Add(long modifierGroupId, long modifierId)
    {
        ModifierMapping mapping = new()
        {
            Modifierid = modifierId,
            Modifiergroupid = modifierGroupId,
            CreatedBy = await _userService.LoggedInUser()
        };

        return await _modifierMappingRepository.AddAsync(mapping);
    }

    public async Task<bool> UpdateModifierGroupMapping(long modifierGroupId, List<long> modifierList)
    {
        List<long> existingModifiersList = _modifierMappingRepository
        .GetByCondition(mm => mm.Modifiergroupid == modifierGroupId && !mm.IsDeleted)
        .Result
        .Select(m => m.Modifierid)
        .ToList();

        // Delete Mapping
        List<long> removeModifiers = existingModifiersList.Except(modifierList).ToList();

        foreach (long modifierId in removeModifiers)
        {
            bool success = await Delete(modifierGroupId, modifierId);
            if (!success)
            {
                return false;
            }
        }

        foreach (long modifierId in modifierList)
        {
            ModifierMapping? existingModifier = await _modifierMappingRepository.GetByStringAsync(mg => mg.Modifiergroupid == modifierGroupId && mg.Modifierid == modifierId && mg.IsDeleted == false);
            if (existingModifier == null)
            {
                bool success = await Add(modifierGroupId, modifierId);
                if (!success)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public async Task<bool> UpdateModifierMapping(long modifierId, List<long> modifierGroupList)
    {
        List<long> existingMgList = _modifierMappingRepository
        .GetByCondition(mm => mm.Modifierid == modifierId && !mm.IsDeleted)
        .Result
        .Select(m => m.Modifiergroupid)
        .ToList();

        // Delete Mapping
        List<long> removeMg = existingMgList.Except(modifierGroupList).ToList();

        foreach (long mgId in removeMg)
        {
            bool success = await Delete(mgId, modifierId);
            if (!success)
            {
                return false;
            }
        }

        foreach (long mgId in modifierGroupList)
        {
            ModifierMapping? mapping = await _modifierMappingRepository.GetByStringAsync(mg => mg.Modifiergroupid == mgId && mg.Modifierid == modifierId && mg.IsDeleted == false);
            if (mapping == null)
            {
                bool success = await Add(mgId, modifierId);
                if (!success)
                {
                    return false;
                }
            }
        }

        return true;
    }

    // Delete all mapping of modifier group 
    public async Task<ResponseViewModel> Delete(long mgId)
    {
        List<ModifierMapping> modifierMappings = _modifierMappingRepository.GetByCondition(mm => mm.Modifiergroupid == mgId).Result.ToList();
        ResponseViewModel response = new();

        foreach (ModifierMapping mapping in modifierMappings)
        {
            response.Success = await Delete(mgId, mapping.Modifierid);
            response.Message = response.Success? NotificationMessages.DeletedFailed.Replace("{0}", "Modifier Group"): NotificationMessages.DeletedFailed.Replace("{0}", "Modifier Group");
            if (!response.Success)
            {
                return response;
            }
        }

        return response;
    }

    public async Task<bool> Delete(long mgId, long modifierId)
    {
        ModifierMapping? mapping = await _modifierMappingRepository.GetByStringAsync(mm => mm.Modifiergroupid == mgId && mm.Modifierid == modifierId && !mm.IsDeleted);
        if (mapping == null)
        {
            return false;
        }

        mapping.IsDeleted = true;
        mapping.UpdatedBy = await _userService.LoggedInUser();
        mapping.UpdatedAt = DateTime.Now;
        return await _modifierMappingRepository.UpdateAsync(mapping);
    }

}

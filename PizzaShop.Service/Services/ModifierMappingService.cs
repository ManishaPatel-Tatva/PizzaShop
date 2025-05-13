using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Common;
using PizzaShop.Service.Exceptions;
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

    public async Task Add(long modifierGroupId, long modifierId)
    {
        ModifierMapping mapping = new()
        {
            Modifierid = modifierId,
            Modifiergroupid = modifierGroupId,
            CreatedBy = await _userService.LoggedInUser()
        };

        await _modifierMappingRepository.AddAsync(mapping);
    }

    public async Task UpdateModifierGroupMapping(long modifierGroupId, List<long> modifierList)
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
            await Delete(modifierGroupId, modifierId);
        }

        foreach (long modifierId in modifierList)
        {
            ModifierMapping? existingModifier = await _modifierMappingRepository.GetByStringAsync(mg => mg.Modifiergroupid == modifierGroupId && mg.Modifierid == modifierId && mg.IsDeleted == false);
            if (existingModifier == null)
            {
                await Add(modifierGroupId, modifierId);
            }
        }
    }

    public async Task UpdateModifierMapping(long modifierId, List<long> modifierGroupList)
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
            await Delete(mgId, modifierId);
        }

        foreach (long mgId in modifierGroupList)
        {
            ModifierMapping? mapping = await _modifierMappingRepository.GetByStringAsync(mg => mg.Modifiergroupid == mgId && mg.Modifierid == modifierId && mg.IsDeleted == false);
            if (mapping == null)
            {
                await Add(mgId, modifierId);
            }
        }
    }

    // Delete all mapping of modifier group 
    public async Task Delete(long mgId)
    {
        List<ModifierMapping> modifierMappings = _modifierMappingRepository.GetByCondition(mm => mm.Modifiergroupid == mgId && !mm.IsDeleted).Result.ToList();

        foreach (ModifierMapping mapping in modifierMappings)
        {
            await Delete(mgId, mapping.Modifierid);
        }
    }

    public async Task Delete(long mgId, long modifierId)
    {
        ModifierMapping mapping = await _modifierMappingRepository.GetByStringAsync(mm => mm.Modifiergroupid == mgId && mm.Modifierid == modifierId && !mm.IsDeleted) ?? throw new NotFoundException(NotificationMessages.NotFound.Replace("{0}", "Modifier Mapping"));

        mapping.IsDeleted = true;
        mapping.UpdatedBy = await _userService.LoggedInUser();
        mapping.UpdatedAt = DateTime.Now;
        
        await _modifierMappingRepository.UpdateAsync(mapping);
    }

}

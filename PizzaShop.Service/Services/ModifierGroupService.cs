using System.Linq.Expressions;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Common;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Services;

public class ModifierGroupService : IModifierGroupService
{
    private readonly IGenericRepository<ModifierGroup> _mgRepository;
    private readonly IUserService _userService;
    private readonly IGenericRepository<ModifierMapping> _modifierMappingRepository;
    private readonly IModifierMappingService _modifierMappingService;

    public ModifierGroupService(IGenericRepository<ModifierGroup> mgRepository, IUserService userService, IGenericRepository<ModifierMapping> modifierMappingRepository, IModifierMappingService modifierMappingService)
    {
        _mgRepository = mgRepository;
        _userService = userService;
        _modifierMappingRepository = modifierMappingRepository;
        _modifierMappingService = modifierMappingService;
    }

    public List<ModifierGroupViewModel> Get()
    {
        List<ModifierGroupViewModel> modifierGroups = _mgRepository.GetByCondition(mg => mg.IsDeleted == false).Result
        .Select(mg => new ModifierGroupViewModel
        {
            Id = mg.Id,
            Name = mg.Name,
            Description = mg.Description
        }).ToList();

        return modifierGroups;
    }

    public async Task<ModifierGroupViewModel> Get(long modifierGroupId)
    {
        if (modifierGroupId == 0)
        {
            return new ModifierGroupViewModel();
        }

        ModifierGroup? modifierGroup = await _mgRepository.GetByIdAsync(modifierGroupId);
        if (modifierGroup == null)
        {
            return new ModifierGroupViewModel();
        }

        ModifierGroupViewModel model = new()
        {
            Id = modifierGroup.Id,
            Name = modifierGroup.Name,
            Description = modifierGroup.Description,

            Modifiers = _modifierMappingRepository.GetByCondition(
                mm => mm.Modifiergroupid == modifierGroupId && !mm.IsDeleted,
                includes: new List<Expression<Func<ModifierMapping, object>>>
                {
                    m => m.Modifier
                }
            ).Result
            .Select(i => new ModifierInfoViewModel
            {
                Id = i.Modifierid,
                Name = i.Modifier.Name
            }).ToList()
        };

        return model;
    }

    public async Task<ResponseViewModel> Save(ModifierGroupViewModel modifierGroupVM)
    {
        ModifierGroup? modifierGroup = await _mgRepository.GetByIdAsync(modifierGroupVM.Id);
        ResponseViewModel response = new();

        modifierGroup ??= new ModifierGroup
        {
            CreatedBy = await _userService.LoggedInUser()
        };

        modifierGroup.Name = modifierGroupVM.Name;
        modifierGroup.Description = modifierGroupVM.Description;
        modifierGroup.UpdatedAt = DateTime.Now;
        modifierGroup.UpdatedBy = await _userService.LoggedInUser();

        if (modifierGroup.Id == 0)
        {
            long mgId = await _mgRepository.AddAsyncReturnId(modifierGroup);

            response.Success = mgId > 1;
            response.Message = mgId > 1 ? NotificationMessages.Added.Replace("{0}", "Modifier Group") : NotificationMessages.AddedFailed.Replace("{0}", "Modifier Group");
        }
        else
        {
            response.Success = await _mgRepository.UpdateAsync(modifierGroup);
            response.Message = response.Success ? NotificationMessages.Updated.Replace("{0}", "Modifier Group") : NotificationMessages.UpdatedFailed.Replace("{0}", "Modifier Group");
        }

        if (!response.Success)
        {
            return response;
        }

        response.Success = await _modifierMappingService.UpdateModifierGroupMapping(modifierGroup.Id, modifierGroupVM.ModifierIdList);
        return response;
    }

    public async Task<ResponseViewModel> Delete(long mgId)
    {
        ModifierGroup? modifierGroup = await _mgRepository.GetByIdAsync(mgId);
        ResponseViewModel response = new();
        if (modifierGroup == null)
        {
            response.Success = false;
            response.Message = NotificationMessages.NotFound.Replace("{0}", "Modifier Group");
            return response;
        }

        modifierGroup.IsDeleted = true;
        modifierGroup.UpdatedAt = DateTime.Now;
        modifierGroup.UpdatedBy = await _userService.LoggedInUser();

        response.Success = await _mgRepository.UpdateAsync(modifierGroup);
        response.Message = response.Success ? NotificationMessages.Deleted.Replace("{0}", "Modifier Group") : NotificationMessages.DeletedFailed.Replace("{0}", "Modifier Group");
        if (!response.Success)
        {
            return response;
        }

        ResponseViewModel mappingResponse = await _modifierMappingService.Delete(mgId);
        if(!mappingResponse.Success)
        {
            return mappingResponse;
        }
        
        return response;
    }

}

using System.Linq.Expressions;
using System.Threading.Tasks;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Common;
using PizzaShop.Service.Exceptions;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Services;

public class ModifierGroupService : IModifierGroupService
{
    private readonly IGenericRepository<ModifierGroup> _mgRepository;
    private readonly IUserService _userService;
    private readonly IGenericRepository<ModifierMapping> _modifierMappingRepository;
    private readonly IModifierMappingService _modifierMappingService;
    private readonly ITransactionRepository _transaction;

    public ModifierGroupService(IGenericRepository<ModifierGroup> mgRepository, IUserService userService, IGenericRepository<ModifierMapping> modifierMappingRepository, IModifierMappingService modifierMappingService, ITransactionRepository transaction)
    {
        _mgRepository = mgRepository;
        _userService = userService;
        _modifierMappingRepository = modifierMappingRepository;
        _modifierMappingService = modifierMappingService;
        _transaction = transaction;

    }

    public async Task<List<ModifierGroupViewModel>> Get()
    {
        IEnumerable<ModifierGroup>? list = await _mgRepository.GetByCondition(
                predicate: mg => mg.IsDeleted == false,
                orderBy: q => q.OrderBy(mg => mg.Id)
        );

        List<ModifierGroupViewModel> modifierGroups = list.Select(mg => new ModifierGroupViewModel
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
        try
        {
            await _transaction.BeginTransactionAsync();

            ModifierGroup modifierGroup = await _mgRepository.GetByIdAsync(modifierGroupVM.Id)
                                            ?? new ModifierGroup
                                            {
                                                CreatedBy = await _userService.LoggedInUser()
                                            };

            ResponseViewModel response = new();

            modifierGroup.Name = modifierGroupVM.Name;
            modifierGroup.Description = modifierGroupVM.Description;
            modifierGroup.UpdatedAt = DateTime.Now;
            modifierGroup.UpdatedBy = await _userService.LoggedInUser();

            if (modifierGroup.Id == 0)
            {
                long mgId = await _mgRepository.AddAsyncReturnId(modifierGroup);

                response.Success = mgId > 1;
                response.Message = mgId > 1 ? NotificationMessages.Added.Replace("{0}", "Modifier Group") : NotificationMessages.AddedFailed.Replace("{0}", "Modifier Group");
                if (!response.Success)
                {
                    return response;
                }

                modifierGroup.Id = mgId;
            }
            else
            {
                await _mgRepository.UpdateAsync(modifierGroup);
                response.Success = true;
                response.Message = NotificationMessages.Updated.Replace("{0}", "Modifier Group");
            }

            await _modifierMappingService.UpdateModifierGroupMapping(modifierGroup.Id, modifierGroupVM.ModifierIdList);

            await _transaction.CommitAsync();

            return response;
        }
        catch
        {
            await _transaction.RollbackAsync();
            throw;
        }
    }


    public async Task Delete(long mgId)
    {
        try
        {
            await _transaction.BeginTransactionAsync();

            ModifierGroup? modifierGroup = await _mgRepository.GetByIdAsync(mgId)
                                        ?? throw new NotFoundException(NotificationMessages.NotFound.Replace("{0}", "Modifier Group"));

            modifierGroup.IsDeleted = true;
            modifierGroup.UpdatedAt = DateTime.Now;
            modifierGroup.UpdatedBy = await _userService.LoggedInUser();

            await _mgRepository.UpdateAsync(modifierGroup);

            await _modifierMappingService.Delete(mgId);

            await _transaction.CommitAsync();
        }
        catch
        {
            await _transaction.RollbackAsync();
            throw;
        }
    }

}

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Common;
using PizzaShop.Service.Helpers;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Services;
public class ModifierService : IModifierService
{
    private readonly IGenericRepository<Modifier> _modifierRepository;
    private readonly IGenericRepository<ModifierGroup> _modifierGroupRepository;
    private readonly IGenericRepository<ModifierMapping> _modifierMappingRepository;
    private readonly IGenericRepository<Unit> _unitRepository;
    private readonly IModifierMappingService _modifierMappingService;
    private readonly IUserService _userService;

    public ModifierService(IGenericRepository<ModifierGroup> modifierGroupRepository, IGenericRepository<Modifier> modifierRepository, IGenericRepository<ModifierMapping> modifierMappingRepository, IGenericRepository<Unit> unitRepository, IModifierMappingService modifierMappingService, IUserService userService)
    {
        _modifierRepository = modifierRepository;
        _modifierGroupRepository = modifierGroupRepository;
        _modifierMappingRepository = modifierMappingRepository;
        _unitRepository = unitRepository;
        _modifierMappingService = modifierMappingService;
        _userService = userService;
    }

    #region  Get
    public async Task<ModifiersPaginationViewModel> Get(long modifierGroupId, int pageSize, int pageNumber, string search)
    {
        IEnumerable<ModifierMapping> modifierMapping = await _modifierMappingRepository.GetByCondition(
            predicate: mm => !mm.IsDeleted &&
                    mm.Modifiergroupid == modifierGroupId &&
                    (string.IsNullOrEmpty(search.ToLower()) ||
                    mm.Modifier.Name.ToLower().Contains(search.ToLower())),
            orderBy: q => q.OrderBy(m => m.Id),
            includes: new List<Expression<Func<ModifierMapping, object>>>
            {
                m => m.Modifier
            },
            thenIncludes: new List<Func<IQueryable<ModifierMapping>, IQueryable<ModifierMapping>>>
            {
                q => q.Include(mm => mm.Modifier)
                    .ThenInclude(m => m.Unit)
            }
        );

        (modifierMapping, int totalRecord) = _modifierMappingRepository.GetPagedRecords(pageSize, pageNumber, modifierMapping);

        ModifiersPaginationViewModel model = new()
        {
            Modifiers = modifierMapping.Select(m => new ModifierViewModel()
            {
                Id = m.Modifierid,
                Name = m.Modifier.Name,
                UnitName = m.Modifier.Unit.Name,
                Rate = m.Modifier.Rate,
                Quantity = m.Modifier.Quantity,
            }).ToList()
        };

        model.Page.SetPagination(totalRecord, pageSize, pageNumber);
        return model;
    }

    public async Task<ModifiersPaginationViewModel> Get(int pageSize, int pageNumber, string search)
    {
        IEnumerable<Modifier> modifiers = await _modifierRepository.GetByCondition(
            predicate: m => !m.IsDeleted &&
                    (string.IsNullOrEmpty(search.ToLower()) ||
                    m.Name.ToLower().Contains(search.ToLower())),
            orderBy: q => q.OrderBy(u => u.Id),
            includes: new List<Expression<Func<Modifier, object>>>
            {
                m => m.Unit
            }
        );

        (modifiers, int totalRecord) = _modifierRepository.GetPagedRecords(pageSize, pageNumber, modifiers);

        ModifiersPaginationViewModel model = new()
        {
            Modifiers = modifiers.Select(m => new ModifierViewModel()
            {
                Id = m.Id,
                Name = m.Name,
                UnitName = m.Unit.Name,
                Rate = m.Rate,
                Quantity = m.Quantity,
            }).ToList()
        };

        model.Page.SetPagination(totalRecord, pageSize, pageNumber);
        return model;
    }

    public async Task<ModifierViewModel> Get(long modifierId)
    {
        ModifierViewModel modifierVM = new()
        {
            ModifierGroups = _modifierGroupRepository.GetAll().ToList(),
            Units = _unitRepository.GetAll().ToList()
        };

        Modifier? modifier = await _modifierRepository.GetByIdAsync(modifierId);
        if (modifier == null)
        {
            return modifierVM;
        }

        modifierVM.Id = modifierId;
        modifierVM.Name = modifier.Name;
        modifierVM.Rate = modifier.Rate;
        modifierVM.Quantity = modifier.Quantity;
        modifierVM.UnitId = modifier.UnitId;
        modifierVM.Description = modifier.Description;
        modifierVM.SelectedMgList = _modifierMappingRepository.GetByCondition(mm => mm.Modifierid == modifierId && !mm.IsDeleted).Result.Select(m => m.Modifiergroupid).ToList();

        return modifierVM;
    }
    #endregion

    #region Save
    public async Task<ResponseViewModel> Save(ModifierViewModel modifierVM)
    {
        Modifier modifier = await _modifierRepository.GetByIdAsync(modifierVM.Id)
                            ?? new Modifier
                            {
                                CreatedBy = await _userService.LoggedInUser()
                            };
                            
        ResponseViewModel response = new();

        modifier.Name = modifierVM.Name;
        modifier.Rate = modifierVM.Rate;
        modifier.Quantity = modifierVM.Quantity;
        modifier.UnitId = modifierVM.UnitId;
        modifier.Description = modifierVM.Description;
        modifier.UpdatedAt = DateTime.Now;
        modifier.UpdatedBy = await _userService.LoggedInUser();

        if (modifier.Id == 0)
        {
            modifier.Id = await _modifierRepository.AddAsyncReturnId(modifier);
            response.Success = modifier.Id > 0;
            response.Message = response.Success ? NotificationMessages.Added.Replace("{0}", "Modifier") : NotificationMessages.AddedFailed.Replace("{0}", "Modifier");
        }
        else
        {
            await _modifierRepository.UpdateAsync(modifier);
            response.Success = true;
            response.Message = response.Success ? NotificationMessages.Updated.Replace("{0}", "Modifier") : NotificationMessages.UpdatedFailed.Replace("{0}", "Modifier");
        }

        await _modifierMappingService.UpdateModifierMapping(modifier.Id, modifierVM.SelectedMgList);

        return response;
    }
    #endregion

    #region Delete
    public async Task Delete(long mgId, List<long> modifierIdList)
    {
        foreach (long modifierId in modifierIdList)
        {
            await _modifierMappingService.Delete(mgId, modifierId);
        }
    }
    #endregion


}
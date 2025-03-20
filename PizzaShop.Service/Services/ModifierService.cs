using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Helpers;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Services;
public class ModifierService : IModifierService
{
    private readonly IGenericRepository<Modifier> _modifierRepository;
    private readonly IGenericRepository<ModifierGroup> _modifierGroupRepository;
    private readonly IGenericRepository<ModifierMapping> _modifierMappingRepository;


    private readonly IGenericRepository<User> _userRepository;

    public ModifierService(IGenericRepository<ModifierGroup> modifierGroupRepository, IGenericRepository<User> userRepository, IGenericRepository<Modifier> modifierRepository, IGenericRepository<ModifierMapping> modifierMappingRepository)
    {
        _modifierRepository = modifierRepository;
        _modifierGroupRepository = modifierGroupRepository;
        _modifierMappingRepository = modifierMappingRepository;
        _userRepository = userRepository;

    }

    #region Modifier Group

    #region Read Modifier Groups
    /*-----------------------------------------------------------Read Modifier Groups---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public List<ModifierGroupViewModel> GetModifierGroups()
    {
        List<ModifierGroupViewModel> modifierGroups = _modifierGroupRepository.GetByCondition(mg => mg.IsDeleted == false)
        .Select(mg => new ModifierGroupViewModel
        {
            ModifierGroupId = mg.Id,
            Name = mg.Name,
            Description = mg.Description
        }).ToList();

        return modifierGroups;
    }

    /*-----------------------------------------------------------Get Modifier Group By Id---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<ModifierGroupViewModel> GetModifierGroup(long modifierGroupId)
    {
        if (modifierGroupId == 0)
        {
            return new ModifierGroupViewModel();
        }

        ModifierGroup modifierGroup = await _modifierGroupRepository.GetByIdAsync(modifierGroupId);

        ModifierGroupViewModel model = new ModifierGroupViewModel
        {
            ModifierGroupId = modifierGroup.Id,
            Name = modifierGroup.Name,
            Description = modifierGroup.Description
        };

        return model;
    }
    #endregion Read Modifier Groups

    #region Add Modifier Groups

    public async Task<bool> SaveModifierGroup(ModifierGroupViewModel model, string createrEmail)
    {
        User creater = await _userRepository.GetByStringAsync(u => u.Email == createrEmail);
        long createrId = creater.Id;

        if (model.ModifierGroupId == 0)
        {
            return await AddModifierGroup(model, createrId);
        }
        else if (model.ModifierGroupId > 0)
        {
            return await UpdateModifierGroup(model, createrId);
        }
        else
        {
            return false;
        }

    }

    public async Task<bool> AddModifierGroup(ModifierGroupViewModel model, long createrId)
    {
        ModifierGroup modifierGroup = new ModifierGroup()
        {
            Name = model.Name,
            Description = model.Description,
            CreatedBy = createrId
        };

        return await _modifierGroupRepository.AddAsync(modifierGroup);
    }
    #endregion Add Modifier Groups

    #region Update Modifier Groups
    public async Task<bool> UpdateModifierGroup(ModifierGroupViewModel model, long createrId)
    {
        ModifierGroup modifierGroup = await _modifierGroupRepository.GetByIdAsync(model.ModifierGroupId);

        modifierGroup.Name = model.Name;
        modifierGroup.Description = model.Description;
        modifierGroup.UpdatedBy = createrId;
        modifierGroup.UpdatedAt = DateTime.Now;

        return await _modifierGroupRepository.UpdateAsync(modifierGroup);
    }

    #endregion Update Modifier Groups

    #region Delete Modifier Group
    /*----------------------------------------------------------------Delete Modifier Group---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<bool> DeleteModifierGroup(long modifierGroupId)
    {
        ModifierGroup modifierGroup = await _modifierGroupRepository.GetByIdAsync(modifierGroupId);

        modifierGroup.IsDeleted = true;

        return await _modifierGroupRepository.UpdateAsync(modifierGroup);
    }

    #endregion Delete Modifier Group

    #endregion Modifier Group

    #region Modifier

    #region Read Modifiers

    public async Task<ModifiersPaginationViewModel> GetPagedModifiers(long modifierGroupId, int pageSize, int pageNumber, string search)
    {
        (IEnumerable<ModifierMapping> modifierMapping, int totalRecord) = await _modifierMappingRepository.GetPagedRecordsAsync(
            pageSize,
            pageNumber,
            filter: mm => !mm.IsDeleted &&
                    mm.Modifiergroupid == modifierGroupId &&
                    (string.IsNullOrEmpty(search.ToLower()) ||
                    mm.Modifier.Name.ToLower().Contains(search.ToLower())),
            orderBy: q => q.OrderBy(u => u.Id),
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

        ModifiersPaginationViewModel model = new() { Page = new() };

        model.Modifiers = modifierMapping.Select(m => new ModifierViewModel()
        {
            ModifierId = m.Modifierid,
            ModifierName = m.Modifier.Name,
            UnitName = m.Modifier.Unit.Name,
            Rate = m.Modifier.Rate,
            Quantity = m.Modifier.Quantity,
        }).ToList();

        model.Page.SetPagination(totalRecord, pageSize, pageNumber);
        return model;
    }

    public async Task<ModifiersPaginationViewModel> GetAllModifiers(int pageSize, int pageNumber, string search)
    {
        (IEnumerable<ModifierMapping> modifierMapping, int totalRecord) = await _modifierMappingRepository.GetPagedRecordsAsync(
            pageSize,
            pageNumber,
            filter: mm => !mm.IsDeleted &&
                    (string.IsNullOrEmpty(search.ToLower()) ||
                    mm.Modifier.Name.ToLower().Contains(search.ToLower())),
            orderBy: q => q.OrderBy(u => u.Id),
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

        ModifiersPaginationViewModel model = new() { Page = new() };

        model.Modifiers = modifierMapping.Select(m => new ModifierViewModel()
        {
            ModifierId = m.Modifierid,
            ModifierName = m.Modifier.Name,
            UnitName = m.Modifier.Unit.Name,
            Rate = m.Modifier.Rate,
            Quantity = m.Modifier.Quantity,
        }).ToList();

        model.Page.SetPagination(totalRecord, pageSize, pageNumber);
        return model;
    }

    /*-----------------------------------------------------------Get Modifier Group By Id---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<ModifierViewModel> GetModifier(long modifierId)
    {
        if (modifierId == 0)
        {
            return new ModifierViewModel();
        }

        Modifier modifier = await _modifierRepository.GetByIdAsync(modifierId);

        ModifierViewModel model = new ModifierViewModel
        {
            ModifierName = modifier.Name,
            Rate = modifier.Rate,
            Quantity = modifier.Quantity,
            UnitId = modifier.UnitId,
            Description = modifier.Description
        };

        return model;
    }

    #endregion Read Modifiers

    #region Add/Update Modifier

    public async Task<bool> SaveModifier(ModifierViewModel model, string createrEmail)
    {
        User creater = await _userRepository.GetByStringAsync(u => u.Email == createrEmail);
        long createrId = creater.Id;

        if (model.ModifierId == 0)
        {
            return await AddModifier(model, createrId);
        }
        else if (model.ModifierId > 0)
        {
            return await UpdateModifier(model, createrId);
        }
        else
        {
            return false;
        }

    }

    #region Add Modifier
    public async Task<bool> AddModifier(ModifierViewModel model, long createrId)
    {
        Modifier modifier = new Modifier()
        {
            Name = model.ModifierName,
            Rate = model.Rate,
            Quantity = model.Quantity,
            UnitId = model.UnitId,
            Description = model.Description,
            CreatedBy = createrId
        };

        return await _modifierRepository.AddAsync(modifier);
    }
    #endregion Add Modifier


    #region Update Modifier
    public async Task<bool> UpdateModifier(ModifierViewModel model, long createrId)
    {
        Modifier modifier = await _modifierRepository.GetByIdAsync(model.ModifierId);

        modifier.Name = model.ModifierName;
        modifier.Rate = model.Rate;
        modifier.Quantity = model.Quantity;
        modifier.UnitId = model.UnitId;
        modifier.Description = model.Description;
        modifier.UpdatedBy = createrId;
        modifier.UpdatedAt = DateTime.Now;

        return await _modifierRepository.UpdateAsync(modifier);
    }

    #endregion Update Modifier 

    #endregion Add/Update Modifier

    #region Delete Modifier 

    /*----------------------------------------------------------------Delete Modifier Group---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<bool> DeleteModifier(long modifierId)
    {
        Modifier modifier = await _modifierRepository.GetByIdAsync(modifierId);

        modifier.IsDeleted = true;

        return await _modifierRepository.UpdateAsync(modifier);
    }

    public async Task<bool> MassDeleteModifiers(List<long> modifierIdList)
    {
        bool success;
        foreach (long id in modifierIdList)
        {
            Modifier modifier = await _modifierRepository.GetByIdAsync(id);

            if (modifier == null)
                return false;

            modifier.IsDeleted = true;
            success = await _modifierRepository.UpdateAsync(modifier);
            if (!success)
                return false;
        }
        return true;
    }

    #endregion Delete Modifier 

    #endregion Modifier

}
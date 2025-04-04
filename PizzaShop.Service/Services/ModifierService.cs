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
    private readonly IGenericRepository<Unit> _unitRepository;

    public ModifierService(IGenericRepository<ModifierGroup> modifierGroupRepository, IGenericRepository<User> userRepository, IGenericRepository<Modifier> modifierRepository, IGenericRepository<ModifierMapping> modifierMappingRepository, IGenericRepository<Unit> unitRepository)
    {
        _modifierRepository = modifierRepository;
        _modifierGroupRepository = modifierGroupRepository;
        _modifierMappingRepository = modifierMappingRepository;
        _userRepository = userRepository;
        _unitRepository = unitRepository;
    }

    #region Modifier Group

    #region Read Modifier Groups
    /*-----------------------------------------------------------Read Modifier Groups---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public List<ModifierGroupViewModel> GetModifierGroups()
    {
        List<ModifierGroupViewModel> modifierGroups = _modifierGroupRepository.GetByCondition(mg => mg.IsDeleted == false).Result
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

        ModifierGroupViewModel model = new ()
        {
            ModifierGroupId = modifierGroup.Id,
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
                ModifierId = i.Modifierid,
                ModifierName = i.Modifier.Name
            }).ToList()
        };

        return model;
    }
    #endregion Read Modifier Groups

    #region Add Modifier Groups
    /*-----------------------------------------------------------Save Modifier Group---------------------------------------------------------------------------------
   ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
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

    /*-----------------------------------------------------------Add Modifier Groups---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<bool> AddModifierGroup(ModifierGroupViewModel model, long createrId)
    {
        ModifierGroup modifierGroup = new ModifierGroup()
        {
            Name = model.Name,
            Description = model.Description,
            CreatedBy = createrId
        };

        long modifierGroupId = await _modifierGroupRepository.AddAsyncReturnId(modifierGroup);

        if (modifierGroupId < 1)
        {
            return false;
        }

        if (modifierGroupId > 0)
        {
            foreach (long modifierId in model.ModifierIdList)
            {
                bool success = await AddModifierMapping(modifierGroupId, modifierId, createrId);
                if (!success)
                    return false;
            }
        }
        return true;

    }

    /*-----------------------------------------------------------Add Modifier Mapping in Modifier Group---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<bool> AddModifierMapping(long modifierGroupId, long modifierId, long createrId)
    {

        ModifierMapping mapping = new ()
        {
            Modifierid = modifierId,
            Modifiergroupid = modifierGroupId,
            CreatedBy = createrId
        };

        return await _modifierMappingRepository.AddAsync(mapping);
    }


    #endregion Add Modifier Groups

    #region Update Modifier Groups
    /*-----------------------------------------------------------Update Modifier Groups---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<bool> UpdateModifierGroup(ModifierGroupViewModel model, long createrId)
    {
        ModifierGroup modifierGroup = await _modifierGroupRepository.GetByIdAsync(model.ModifierGroupId);

        modifierGroup.Name = model.Name;
        modifierGroup.Description = model.Description;
        modifierGroup.UpdatedBy = createrId;
        modifierGroup.UpdatedAt = DateTime.Now;

        bool success = await _modifierGroupRepository.UpdateAsync(modifierGroup);
        if (!success)
            return false;

        List<long> modifierList = model.ModifierIdList;

        return await UpdateModifierMapping(model.ModifierGroupId, modifierList, createrId);
    }

    /*-----------------------------------------------------------Update Modifier Mapping in Modifier Group---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<bool> UpdateModifierMapping(long modifierGroupId, List<long> modifierList, long createrId)
    {
        List<long> existingModifiersList = _modifierMappingRepository
        .GetByCondition(mm => mm.Modifiergroupid == modifierGroupId && !mm.IsDeleted)
        .Result
        .Select(m => m.Modifierid)
        .ToList();


        List<long> removeModifiers = existingModifiersList.Except(modifierList).ToList();

        foreach (long modifierId in removeModifiers)
        {
            ModifierMapping mapping = await _modifierMappingRepository
            .GetByStringAsync(mm => mm.Modifiergroupid == modifierGroupId && mm.Modifierid == modifierId && !mm.IsDeleted);

            mapping.IsDeleted = true;
            mapping.UpdatedBy = createrId;
            mapping.UpdatedAt = DateTime.Now;

            bool success = await _modifierMappingRepository.UpdateAsync(mapping);
            if (!success)
                return false;
        }

        foreach (long modifierId in modifierList)
        {
            ModifierMapping existingModifier = await _modifierMappingRepository.GetByStringAsync(mg => mg.Modifiergroupid == modifierGroupId && mg.Modifierid == modifierId && mg.IsDeleted == false);
            if (existingModifier == null)
            {
                bool success = await AddModifierMapping(modifierGroupId, modifierId, createrId);
                if (!success)
                    return false;
            }
        }

        return true;
    }

    #endregion Update Modifier Groups

    #region Delete Modifier Group
    /*----------------------------------------------------------------Delete Modifier Group---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<bool> DeleteModifierGroup(long modifierGroupId, string createrEmail)
    {
        User user = await _userRepository.GetByStringAsync(u => u.Email == createrEmail);
        ModifierGroup modifierGroup = await _modifierGroupRepository.GetByIdAsync(modifierGroupId);
        modifierGroup.IsDeleted = true;
        bool success = await _modifierGroupRepository.UpdateAsync(modifierGroup);

        if (!success)
            return false;

        List<ModifierMapping> modifierMappings = _modifierMappingRepository.GetByCondition(mm => mm.Modifiergroupid == modifierGroupId).Result.ToList();

        foreach (ModifierMapping mapping in modifierMappings)
        {
            mapping.IsDeleted = true;
            mapping.UpdatedBy = user.Id;
            mapping.UpdatedAt = DateTime.Now;
            success = await _modifierMappingRepository.UpdateAsync(mapping);
            if (!success)
                return false;
        }

        return success;


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
            predicate: mm => !mm.IsDeleted &&
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
        (IEnumerable<Modifier> modifiers, int totalRecord) = await _modifierRepository.GetPagedRecordsAsync(
            pageSize,
            pageNumber,
            predicate: m => !m.IsDeleted &&
                    (string.IsNullOrEmpty(search.ToLower()) ||
                    m.Name.ToLower().Contains(search.ToLower())),
            orderBy: q => q.OrderBy(u => u.Id),
            includes: new List<Expression<Func<Modifier, object>>>
            {
                m => m.Unit
            }
        );

        ModifiersPaginationViewModel model = new() { Page = new() };

        model.Modifiers = modifiers.Select(m => new ModifierViewModel()
        {
            ModifierId = m.Id,
            ModifierName = m.Name,
            UnitName = m.Unit.Name,
            Rate = m.Rate,
            Quantity = m.Quantity,
        }).ToList();

        model.Page.SetPagination(totalRecord, pageSize, pageNumber);
        return model;
    }

    /*-----------------------------------------------------------Get Modifier By Id---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<ModifierViewModel> GetModifier(long modifierId)
    {
        ModifierViewModel model = new ModifierViewModel
        {
            ModifierGroups = _modifierGroupRepository.GetAll().ToList(),
            Units = _unitRepository.GetAll().ToList()
        };

        if (modifierId == 0)
        {
            return model;
        }

        Modifier modifier = await _modifierRepository.GetByIdAsync(modifierId);
        
        model.ModifierId = modifierId;
        model.ModifierName = modifier.Name;
        model.Rate = modifier.Rate;
        model.Quantity = modifier.Quantity;
        model.UnitId = modifier.UnitId;
        model.Description = modifier.Description;
        model.SelectedMgList =  _modifierMappingRepository.GetByCondition(mm => mm.Modifierid == modifierId && !mm.IsDeleted).Result.Select(m => m.Modifiergroupid).ToList();

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
        Modifier modifier = new ()
        {
            Name = model.ModifierName,
            Rate = model.Rate,
            Quantity = model.Quantity,
            UnitId = model.UnitId,
            FoodTypeId = 2,
            Description = model.Description,
            CreatedBy = createrId
        };

        long modifierId = await _modifierRepository.AddAsyncReturnId(modifier);

        if (modifierId < 1)
        {
            return false;
        }

        if (modifierId > 0)
        {
            foreach (long mgId in model.SelectedMgList)
            {
                bool success = await AddModifierMapping(mgId, modifierId, createrId);
                if (!success)
                    return false;
            }
        }
        return true;
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

        bool success = await _modifierRepository.UpdateAsync(modifier);
        if (!success)
            return false;

        return await UpdateModifierGroupMapping( model.ModifierId, model.SelectedMgList ,  createrId);

    }

    public async Task<bool> UpdateModifierGroupMapping(long modifierId, List<long> modifierGroupList, long createrId)
    {
        List<long> existingMgList = _modifierMappingRepository
        .GetByCondition(mm => mm.Modifierid == modifierId && !mm.IsDeleted)
        .Result
        .Select(m => m.Modifiergroupid)
        .ToList();

        List<long> removeMg = existingMgList.Except(modifierGroupList).ToList();

        foreach (long mgId in removeMg)
        {
            ModifierMapping mapping =  await _modifierMappingRepository
            .GetByStringAsync(mm => mm.Modifiergroupid == mgId && mm.Modifierid == modifierId && !mm.IsDeleted);

            mapping.IsDeleted = true;
            mapping.UpdatedBy = createrId;
            mapping.UpdatedAt = DateTime.Now;

            bool success = await _modifierMappingRepository.UpdateAsync(mapping);
            if (!success)
                return false;
        }

        foreach (long mgId in modifierGroupList)
        {
            ModifierMapping existingModifierGroup = await _modifierMappingRepository.GetByStringAsync(mg => mg.Modifiergroupid == mgId && mg.Modifierid == modifierId && mg.IsDeleted == false);
            if (existingModifierGroup == null)
            {
                bool success = await AddModifierMapping(mgId, modifierId, createrId);
                if (!success)
                    return false;
            }
        }

        return true;
    }

    #endregion Update Modifier 

    #endregion Add/Update Modifier

    #region Delete Modifier 

    /*----------------------------------------------------------------Delete Modifier Group---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<bool> DeleteModifier(long modifierId, long modifierGroupId, string createrEmail)
    {
        User user = await _userRepository.GetByStringAsync(u => u.Email == createrEmail);
        if (user == null)
            return false;

        ModifierMapping mapping = await _modifierMappingRepository.GetByStringAsync(mm => mm.Modifierid == modifierId && mm.Modifiergroupid == modifierGroupId);
        if (mapping == null)
            return false;

        mapping.IsDeleted = true;
        mapping.UpdatedBy = user.Id;
        mapping.UpdatedAt = DateTime.Now;

        return await _modifierMappingRepository.UpdateAsync(mapping);
    }

    public async Task<bool> MassDeleteModifiers(List<long> modifierIdList, long modifierGroupId, string createrEmail)
    {
        User user = await _userRepository.GetByStringAsync(u => u.Email == createrEmail);

        foreach (long modifierId in modifierIdList)
        {
            ModifierMapping mapping = await _modifierMappingRepository.GetByStringAsync(mm => mm.Modifierid == modifierId && mm.Modifiergroupid == modifierGroupId);

            if (mapping == null)
                return false;

            mapping.IsDeleted = true;
            mapping.UpdatedBy = user.Id;
            mapping.UpdatedAt = DateTime.Now;
            bool success = await _modifierMappingRepository.UpdateAsync(mapping);
            if (!success)
                return false;
        }
        return true;
    }


    #endregion Delete Modifier 

    #endregion Modifier



}
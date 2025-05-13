using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Common;
using PizzaShop.Service.Exceptions;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Services;

public class ItemModifierService : IItemModifierService
{
    private readonly IGenericRepository<ItemModifierGroup> _itemModifierGroupRepository;
    private readonly IGenericRepository<ModifierGroup> _modifierGroupRepository;
    private readonly IUserService _userService;


    public ItemModifierService(IGenericRepository<ItemModifierGroup> itemModifierGroupRepository, IUserService userService, IGenericRepository<ModifierGroup> modifierGroupRepository)
    {
        _itemModifierGroupRepository = itemModifierGroupRepository;
        _userService = userService;
        _modifierGroupRepository = modifierGroupRepository;
    }

    #region Get

    /*-----------------------------------------------------------Get Modifier on Selection---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<ItemModifierViewModel> Get(long modifierGroupId)
    {
        IEnumerable<ModifierGroup> list = await _modifierGroupRepository.GetByCondition(
            m => m.Id == modifierGroupId && !m.IsDeleted,
            includes: new List<Expression<Func<ModifierGroup, object>>>
            {
                mg => mg.ModifierMappings,
                mg => mg.ItemModifierGroups

            },
            thenIncludes: new List<Func<IQueryable<ModifierGroup>, IQueryable<ModifierGroup>>>
            {
                q => q.Include(mg => mg.ModifierMappings)
                    .ThenInclude(m => m.Modifier) // Deepest level include
            });

        ItemModifierViewModel modifierGroups = list.Select(mg => new ItemModifierViewModel
        {
            ModifierGroupId = mg.Id,
            ModifierGroupName = mg.Name,
            ModifierList = mg.ModifierMappings
                .Where(i => !i.IsDeleted)
                .Select(mm => new ModifierViewModel
                {
                    Id = mm.Modifier.Id,
                    Name = mm.Modifier.Name,
                    Rate = mm.Modifier.Rate
                }).ToList()
        }).FirstOrDefault() ?? throw new NotFoundException(NotificationMessages.NotFound.Replace("{0}", "Item"));

        return modifierGroups;
    }

    public async Task<List<ItemModifierViewModel>> List(long itemId)
    {
        IEnumerable<ItemModifierGroup>? mappings = await _itemModifierGroupRepository.GetByCondition(
            i => i.ItemId == itemId && !i.IsDeleted,
            includes: new List<Expression<Func<ItemModifierGroup, object>>>
            {
                img => img.ModifierGroup
            },
            thenIncludes: new List<Func<IQueryable<ItemModifierGroup>, IQueryable<ItemModifierGroup>>>
            {
                q => q.Include(img => img.ModifierGroup)
                    .ThenInclude(mg => mg.ModifierMappings)
                    .ThenInclude(m => m.Modifier) // Deepest level include
            }
            );

        List<ItemModifierViewModel> list = mappings.Select(i => new ItemModifierViewModel
        {
            ModifierGroupId = i.ModifierGroupId,
            ModifierGroupName = i.ModifierGroup.Name,
            MinAllowed = i.MinAllowed,
            MaxAllowed = i.MaxAllowed,
            ModifierList = i.ModifierGroup.ModifierMappings
                .Where(i => !i.IsDeleted)
                .Select(m => new ModifierViewModel
                {
                    Id = m.Modifier.Id,
                    Name = m.Modifier.Name,
                    Rate = m.Modifier.Rate
                }).ToList()
        }).ToList();

        return list;
    }

    #endregion

    #region  Save
    public async Task Save(ItemModifierViewModel itemModifierVM)
    {
        ItemModifierGroup mapping = await _itemModifierGroupRepository.GetByStringAsync(
            mg => mg.ItemId == itemModifierVM.ItemId
            && mg.ModifierGroupId == itemModifierVM.ModifierGroupId
            && mg.IsDeleted == false)
            ?? new ItemModifierGroup()
            {
                ItemId = itemModifierVM.ItemId,
                ModifierGroupId = itemModifierVM.ModifierGroupId,
                CreatedBy = await _userService.LoggedInUser()
            };

        mapping.MinAllowed = itemModifierVM.MinAllowed;
        mapping.MaxAllowed = itemModifierVM.MaxAllowed;
        mapping.UpdatedAt = DateTime.Now;
        mapping.UpdatedBy = await _userService.LoggedInUser();

        if (mapping.Id == 0)
        {
            await _itemModifierGroupRepository.AddAsync(mapping);
        }
        else
        {
            await _itemModifierGroupRepository.UpdateAsync(mapping);
        }

    }

    public async Task Save(long itemId, List<ItemModifierViewModel> itemModifierList)
    {
        List<long> existingGroupIds = _itemModifierGroupRepository
        .GetByCondition(m => m.ItemId == itemId && !m.IsDeleted)
        .Result
        .Select(mg => mg.ModifierGroupId)
        .ToList();

        List<long> currentGroupIds = itemModifierList.Select(mg => mg.ModifierGroupId).ToList();

        List<long> removeGroupIds = existingGroupIds.Except(currentGroupIds).ToList();

        foreach (long groupId in removeGroupIds)
        {
            await Delete(itemId, groupId);
        }

        foreach (ItemModifierViewModel itemModifierVM in itemModifierList)
        {
            itemModifierVM.ItemId = itemId;
            await Save(itemModifierVM);
        }
    }

    #endregion Save

    #region Delete
    public async Task Delete(long itemId, long modifierGroupId)
    {
        ItemModifierGroup mapping = await _itemModifierGroupRepository.GetByStringAsync(mg => mg.ModifierGroupId == modifierGroupId
                                    && mg.ItemId == itemId && !mg.IsDeleted)
                                    ?? throw new NotFoundException(NotificationMessages.NotFound.Replace("{0}", "Item"));

        mapping.IsDeleted = true;
        mapping.UpdatedAt = DateTime.Now;
        mapping.UpdatedBy = await _userService.LoggedInUser();

        await _itemModifierGroupRepository.UpdateAsync(mapping);
    }
    #endregion Delete
}

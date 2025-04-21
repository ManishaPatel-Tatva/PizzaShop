using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Common;
using PizzaShop.Service.Helpers;
using PizzaShop.Service.Interfaces;
using Table = PizzaShop.Entity.Models.Table;

namespace PizzaShop.Service.Services;

public class TableService : ITableService
{
    private readonly IGenericRepository<Table> _tableRepository;
    private readonly IGenericRepository<Section> _sectionRepository;
    private readonly IGenericRepository<TableStatus> _tableStatusRepository;
    private readonly IUserService _userService;

    public TableService(IGenericRepository<Table> tableRepository, IGenericRepository<Section> sectionRepository, IGenericRepository<TableStatus> tableStatusRepository, IUserService userService)
    {
        _tableRepository = tableRepository;
        _sectionRepository = sectionRepository;
        _tableStatusRepository = tableStatusRepository;
        _userService = userService;

    }


    #region Get
    /*-----------------------------------------------------------------Display Tables---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<TablesPaginationViewModel> Get(long sectionId, FilterViewModel filter)
    {
        IEnumerable<Table> tables = await _tableRepository.GetByCondition(
            predicate: t => !t.IsDeleted &&
                    t.SectionId == sectionId &&
                    (string.IsNullOrEmpty(filter.Search) ||
                    t.Name.ToLower().Contains(filter.Search.ToLower())),
            orderBy: q => q.OrderBy(u => u.Id),
            includes: new List<Expression<Func<Table, object>>> { u => u.Section, u => u.Status }
        );

        (tables, int totalRecord) = await _tableRepository.GetPagedRecords(filter.PageSize, filter.PageNumber, tables);

        TablesPaginationViewModel tablesVM = new()
        {
            Page = new(),
            Tables = tables.Select(t => new TableViewModel()
            {
                Id = t.Id,
                Name = t.Name,
                Capacity = t.Capacity,
                StatusName = t.Status.Name,
            }).ToList()
        };

        tablesVM.Page.SetPagination(totalRecord, filter.PageSize, filter.PageNumber);
        return tablesVM;
    }

    /*-----------------------------------------------------------Get Table By Id---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<TableViewModel> Get(long tableId)
    {
        TableViewModel tableVM = new()
        {
            SectionList = _sectionRepository.GetAll().ToList(),
            StatusList = _tableStatusRepository.GetAll().ToList()
        };

        if (tableId == 0)
        {
            return tableVM;
        }
        else
        {
            Table? table = await _tableRepository.GetByIdAsync(tableId);

            tableVM.Id = tableId;
            tableVM.Name = table.Name;
            tableVM.SectionId = table.SectionId;
            tableVM.Capacity = table.Capacity;
            tableVM.StatusId = table.StatusId;

            return tableVM;
        }
    }

    #endregion Get

    #region Save

    public async Task<ResponseViewModel> Save(TableViewModel tableVM)
    {
        long createrId = await _userService.LoggedInUser();

        Table? table = new();
        ResponseViewModel response = new();

        if (tableVM.Id == 0)
        {
            table.CreatedBy = createrId;
            table.SectionId = tableVM.SectionId;
            table.StatusId = tableVM.StatusId;
        }
        else if (tableVM.Id > 0)
        {
            table = await _tableRepository.GetByIdAsync(tableVM.Id);
        }
        else
        {
            response.Success = false;
            response.Message = NotificationMessages.NotFound.Replace("{0}", "Table");
            return response;
        }

        table.Name = tableVM.Name;
        table.Capacity = tableVM.Capacity;
        table.UpdatedBy = createrId;
        table.UpdatedAt = DateTime.Now;

        if (tableVM.Id == 0)
        {
            response.Success = await _tableRepository.AddAsync(table);
            response.Message = response.Success ? NotificationMessages.Added.Replace("{0}", "Section") : NotificationMessages.AddedFailed.Replace("{0}", "Section");
        }
        else
        {
            response.Success = await _tableRepository.UpdateAsync(table);
            response.Message = response.Success ? NotificationMessages.Updated.Replace("{0}", "Section") : NotificationMessages.UpdatedFailed.Replace("{0}", "Section");
        }

        return response;

    }

    public async Task<bool> AssignTable(long tableId)
    {
        Table? table = await _tableRepository.GetByIdAsync(tableId);
        
        if (table == null)
        {
            return false;
        }

        table.StatusId = _tableStatusRepository.GetByStringAsync(s => s.Name == "Assigned").Result.Id;

        return await _tableRepository.UpdateAsync(table);
    }

    #endregion Save

    #region Delete
    /*----------------------------------------------------------------Delete Table Group---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<ResponseViewModel> Delete(long tableId)
    {
        Table? table = await _tableRepository.GetByIdAsync(tableId);
        ResponseViewModel response = new();
        if (table == null)
        {
            response.Success = false;
            response.Message = NotificationMessages.NotFound.Replace("{0}", "Table");
            return response;
        }

        table.IsDeleted = true;
        table.UpdatedBy = await _userService.LoggedInUser();
        table.UpdatedAt = DateTime.Now;

        response.Success = await _tableRepository.UpdateAsync(table);
        response.Message = response.Success ? NotificationMessages.Deleted.Replace("{0}", "Table") : NotificationMessages.DeletedFailed.Replace("{0}", "Table");
        return response;
    }

    public async Task<ResponseViewModel> Delete(List<long> tableIdList)
    {
        ResponseViewModel response = new();
        foreach (long id in tableIdList)
        {
            response = await Delete(id);
            if (!response.Success)
            {
                return response;
            }
        }
        response.Success = true;
        response.Message = NotificationMessages.Deleted.Replace("{0}", "Tables");
        return response;
    }

    #endregion Delete 

}

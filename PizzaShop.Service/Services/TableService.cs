using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Common;
using PizzaShop.Service.Exceptions;
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

        (tables, int totalRecord) = _tableRepository.GetPagedRecords(filter.PageSize, filter.PageNumber, tables);

        TablesPaginationViewModel tablesVM = new()
        {
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


    /*-----------------------------------------------------------------Display Tables---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<List<TableViewModel>> List(long sectionId)
    {
        IEnumerable<Table> list = await _tableRepository.GetByCondition(
            predicate: t => !t.IsDeleted &&
                    t.SectionId == sectionId,
            orderBy: q => q.OrderBy(t => t.Id),
            includes: new List<Expression<Func<Table, object>>> { u => u.Status }
        );

        List<TableViewModel> tables = list.Select(t => new TableViewModel()
        {
            Id = t.Id,
            Name = t.Name,
            Capacity = t.Capacity,
            StatusName = t.Status.Name,
        }).ToList();

        return tables;
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

        Table? table = await _tableRepository.GetByIdAsync(tableId);

        if (table == null)
        {
            return tableVM;
        }

        tableVM.Id = tableId;
        tableVM.Name = table.Name;
        tableVM.SectionId = table.SectionId;
        tableVM.Capacity = table.Capacity;
        tableVM.StatusId = table.StatusId;

        return tableVM;

    }

    #endregion Get

    #region Save

    public async Task<ResponseViewModel> Save(TableViewModel tableVM)
    {
        long createrId = await _userService.LoggedInUser();

        Table table = await _tableRepository.GetByIdAsync(tableVM.Id)
                    ?? new()
                    {
                        CreatedBy = createrId,
                        SectionId = tableVM.SectionId,
                        StatusId = tableVM.StatusId
                    };

        ResponseViewModel response = new();


        table.Name = tableVM.Name;
        table.Capacity = tableVM.Capacity;
        table.UpdatedBy = createrId;
        table.UpdatedAt = DateTime.Now;

        if (tableVM.Id == 0)
        {
            await _tableRepository.AddAsync(table);
            response.Success = true;
            response.Message = NotificationMessages.Added.Replace("{0}", "Section");
        }
        else
        {
            await _tableRepository.UpdateAsync(table);
            response.Message = NotificationMessages.Updated.Replace("{0}", "Section");
        }

        return response;
    }

    #endregion Save

    #region Table Status

    public async Task ChangeStatus(long tableId, string status)
    {
        Table table = await _tableRepository.GetByIdAsync(tableId)
                    ?? throw new NotFoundException(NotificationMessages.NotFound.Replace("{0}", "Table"));

        TableStatus tableStatus = await _tableStatusRepository.GetByStringAsync(s => s.Name == status)
                                ?? throw new NotFoundException(NotificationMessages.NotFound.Replace("{0}", "Table"));
        
        table.StatusId = tableStatus.Id;
        table.UpdatedBy = await _userService.LoggedInUser();
        table.UpdatedAt = DateTime.Now;

        await _tableRepository.UpdateAsync(table);
    }

    #endregion

    #region Delete
    /*----------------------------------------------------------------Delete Table Group---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task Delete(long tableId)
    {
        Table table = await _tableRepository.GetByIdAsync(tableId)
                    ?? throw new NotFoundException(NotificationMessages.NotFound.Replace("{0}", "Table"));

        table.IsDeleted = true;
        table.UpdatedBy = await _userService.LoggedInUser();
        table.UpdatedAt = DateTime.Now;

        await _tableRepository.UpdateAsync(table);
    }

    public async Task Delete(List<long> tableIdList)
    {
        foreach (long id in tableIdList)
        {
            await Delete(id);
        }
    }

    // public async Task CascadeDelete(long sectionId)
    // {
    //     TableViewModel list = await Get(sectionId, new FilterViewModel());
    //     foreach(var table in list)
    // }

    #endregion Delete 

}

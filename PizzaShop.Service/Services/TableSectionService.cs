using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Helpers;
using PizzaShop.Service.Interfaces;
using Table = PizzaShop.Entity.Models.Table;

namespace PizzaShop.Service.Services;

public class TableSectionService : ITableSectionService
{
    private readonly IGenericRepository<Entity.Models.Table> _tableRepository;
    private readonly IGenericRepository<Section> _sectionRepository;
    private readonly IGenericRepository<TableStatus> _tableStatusRepository;
    private readonly IGenericRepository<User> _userRepository;

    public TableSectionService(IGenericRepository<Entity.Models.Table> tableRepository, IGenericRepository<Section> sectionRepository, IGenericRepository<User> userRepository, IGenericRepository<TableStatus> tableStatusRepository)
    {
        _tableRepository = tableRepository;
        _sectionRepository = sectionRepository;
        _userRepository = userRepository;
        _tableStatusRepository = tableStatusRepository;
    }

    #region Section

    #region Read Section
    /*-----------------------------------------------------------Read Section---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public List<SectionViewModel> GetAllSections()
    {
        List<SectionViewModel> sections = _sectionRepository.GetByCondition(sec => sec.IsDeleted == false)
        .Select(s => new SectionViewModel
        {
            SectionId = s.Id,
            Name = s.Name,
        }).ToList();

        return sections;
    }

    /*-----------------------------------------------------------Get Section Group By Id---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<SectionViewModel> GetSection(long sectionId)
    {
        if (sectionId == 0)
        {
            return new SectionViewModel();
        }

        Section section = await _sectionRepository.GetByIdAsync(sectionId);

        SectionViewModel model = new SectionViewModel
        {
            SectionId = section.Id,
            Name = section.Name,
            Description = section.Description
        };

        return model;
    }

    #endregion Read Section

    #region Add/Update Section

    public async Task<bool> SaveSection(SectionViewModel model, string createrEmail)
    {
        User creater = await _userRepository.GetByStringAsync(u => u.Email == createrEmail);
        long createrId = creater.Id;

        if (model.SectionId == 0)
        {
            return await AddSection(model, createrId);
        }
        else if (model.SectionId > 0)
        {
            return await UpdateSection(model, createrId);
        }
        else
        {
            return false;
        }

    }

    public async Task<bool> AddSection(SectionViewModel model, long createrId)
    {
        Section section = new Section()
        {
            Name = model.Name,
            Description = model.Description,
            CreatedBy = createrId
        };

        return await _sectionRepository.AddAsync(section);
    }

    public async Task<bool> UpdateSection(SectionViewModel model, long createrId)
    {
        Section section = await _sectionRepository.GetByIdAsync(model.SectionId);

        section.Name = model.Name;
        section.Description = model.Description;
        section.UpdatedBy = createrId;
        section.UpdatedAt = DateTime.Now;

        return await _sectionRepository.UpdateAsync(section);
    }

    #endregion Add/Update Section

    #region Delete Section
    /*----------------------------------------------------------------Delete Section Group---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<bool> DeleteSection(long sectionId)
    {
        Section section = await _sectionRepository.GetByIdAsync(sectionId);

        section.IsDeleted = true;

        return await _sectionRepository.UpdateAsync(section);
    }

    #endregion Delete Section

    #endregion Section

    #region Table

    #region Display Tables
    /*-----------------------------------------------------------------Display Tables---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<TablesPaginationViewModel> GetPagedTables(long sectionId, int pageSize, int pageNumber, string search)
    {
        (IEnumerable<Table> tables, int totalRecord) = await _tableRepository.GetPagedRecordsAsync(
            pageSize,
            pageNumber,
            filter: t => !t.IsDeleted &&
                    t.SectionId == sectionId &&
                    (string.IsNullOrEmpty(search.ToLower()) ||
                    t.Name.ToLower().Contains(search.ToLower())),
            orderBy: q => q.OrderBy(u => u.Id),
            includes: new List<Expression<Func<Table, object>>> { u => u.Section, u=> u.Status }
        );

        TablesPaginationViewModel model = new() { Page = new() };

        model.Tables = tables.Select(t => new TableViewModel()
        {
            TableId = t.Id,
            Name = t.Name,
            Capacity = t.Capacity,
            StatusName = t.Status.Name,
        }).ToList();

        model.Page.SetPagination(totalRecord, pageSize, pageNumber);
        return model;
    }

    /*-----------------------------------------------------------Get Table By Id---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<TableViewModel> GetTable(long tableId)
    {
        TableViewModel model = new TableViewModel
        {
            SectionList = _sectionRepository.GetAll().ToList(),
            StatusList = _tableStatusRepository.GetAll().ToList()
        };

        if (tableId == 0)
            return model;

        Table table = await _tableRepository.GetByIdAsync(tableId);
        
        model.Name = table.Name;
        model.SectionId = table.SectionId;
        model.Capacity = table.Capacity;
        model.StatusId = table.StatusId;
        
        return model;
    }

    #endregion Display Tables

     #region Add/Update Table

    public async Task<bool> SaveTable(TableViewModel model, string createrEmail)
    {
        User creater = await _userRepository.GetByStringAsync(u => u.Email == createrEmail);
        long createrId = creater.Id;

        if (model.TableId == 0)
        {
            return await AddTable(model, createrId);
        }
        else if (model.TableId > 0)
        {
            return await UpdateTable(model, createrId);
        }
        else
        {
            return false;
        }

    }

    #region Add Table
    public async Task<bool> AddTable(TableViewModel model, long createrId)
    {
        Table table = new Table()
        {
            Name = model.Name,
            SectionId = model.SectionId,
            Capacity = model.Capacity,
            StatusId = model.StatusId,
            
            CreatedBy = createrId
        };

        return await _tableRepository.AddAsync(table);
    }
    #endregion Add Table


    #region Update Table
    public async Task<bool> UpdateTable(TableViewModel model, long createrId)
    {
        Table table = await _tableRepository.GetByIdAsync(model.TableId);

        if (table == null)
            return false;

        table.Name = model.Name;
        table.Capacity = model.Capacity;
        table.UpdatedBy = createrId;
        table.UpdatedAt = DateTime.Now;

        return await _tableRepository.UpdateAsync(table);
    }

    #endregion Update Table 

    #endregion Add/Update Table

    #region Delete Table 

    /*----------------------------------------------------------------Delete Table Group---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<bool> DeleteTable(long tableId)
    {
        Table table = await _tableRepository.GetByIdAsync(tableId);

        table.IsDeleted = true;

        return await _tableRepository.UpdateAsync(table);
    }

    public async Task<bool> MassDeleteTables(List<long> tableIdList)
    {
        bool success;
        foreach (long id in tableIdList)
        {
            Table table = await _tableRepository.GetByIdAsync(id);

            if (table == null)
                return false;

            table.IsDeleted = true;
            success = await _tableRepository.UpdateAsync(table);
            if (!success)
                return false;
        }
        return true;
    }

    #endregion Delete Table 


    #endregion Table
}

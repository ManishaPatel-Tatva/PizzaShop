using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Common;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Services;

public class AppTableService : IAppTableService
{
    private readonly IGenericRepository<Section> _sectionRepository;
    private readonly IGenericRepository<OrderTableMapping> _orderTableRepository;
    private readonly IWaitingListService _waitingService;
    private readonly IUserService _userService;
    private readonly ITableService _tableService;

    public AppTableService(IGenericRepository<Section> sectionRepository, IGenericRepository<OrderTableMapping> orderTableRepository, IWaitingListService waitingService, IUserService userService, ITableService tableService)
    {
        _sectionRepository = sectionRepository;
        _orderTableRepository = orderTableRepository;
        _waitingService = waitingService;
        _userService = userService;
        _tableService = tableService;
    }

    #region  Get

    public async Task<List<SectionViewModel>> Get()
    {
        try
        {
            IEnumerable<Section>? list = await _sectionRepository.GetByCondition(
                s => !s.IsDeleted,
                thenIncludes: new List<Func<IQueryable<Section>, IQueryable<Section>>>
                {
                q => q.Include(s => s.Tables)
                    .ThenInclude(t => t.Status),
                q => q.Include(s => s.Tables)
                    .ThenInclude(t => t.OrderTableMappings)
                    .ThenInclude(otm => otm.Order)
                }
            );

            List<SectionViewModel> sections = list.Select(s => new SectionViewModel
            {
                Id = s.Id,
                Name = s.Name,
                Tables = s.Tables.Select(t => new TableCardViewModel
                {
                    Id = t.Id,
                    TableName = t.Name,
                    TableStatus = t.Status.Name,
                    OrderAmount = t.OrderTableMappings.Where(otm => otm.TableId == t.Id && !otm.IsDeleted).Any(otm => otm.OrderId == null)  ?  0 :
                                t.OrderTableMappings.Where(otm => otm.TableId == t.Id && !otm.IsDeleted).Select(otm => otm.Order.FinalAmount).FirstOrDefault(),
                    OrderTime = t.OrderTableMappings
                                .Where(otm => otm.TableId == t.Id && !otm.IsDeleted)
                                .Select(otm => otm.CreatedAt)
                                .FirstOrDefault(),
                    Capacity = t.Capacity
                }).ToList()
            }).ToList();

            return sections;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    #endregion Get

    #region  Add

    public async Task<ResponseViewModel> Add(AssignTableViewModel assignTableVM)
    {
        //Add Waiting Token
        ResponseViewModel response = await _waitingService.Save(assignTableVM.WaitingToken);
        if(!response.Success)
        {
            return response;
        }

        //Assign Table
        foreach(TableViewModel? table in assignTableVM.Tables)
        {
            OrderTableMapping mapping = new()
            {
                TableId = table.Id,
                CustomerId = assignTableVM.WaitingToken.CustomerId,
                CreatedBy = await _userService.LoggedInUser()
            };

            //Change table Status from available to assigned
            response.Success = await _tableService.AssignTable(table.Id);
            if(!response.Success)
            {
                response.Message = NotificationMessages.Failed.Replace("{0}", "Table assignment");
                return response;
            }

            //Order Table Mappping
            response.Success = await _orderTableRepository.AddAsync(mapping);
            if(!response.Success)
            {
                response.Message = NotificationMessages.Failed.Replace("{0}", "Table assignment");
                return response;
            }
        }

        response.Success = await _waitingService.AssignTable(response.EntityId);
        response.Message = response.Success ? NotificationMessages.Successfully.Replace("{0}", "Table Assigned") : NotificationMessages.Failed.Replace("{0}", "Table assignment");
        return response;
    }

    #endregion Add

    

}

using Microsoft.EntityFrameworkCore;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Common;
using PizzaShop.Service.Exceptions;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Services;

public class AppTableService : IAppTableService
{
    private readonly IGenericRepository<Section> _sectionRepository;
    private readonly IGenericRepository<OrderTableMapping> _orderTableRepository;
    private readonly IWaitingListService _waitingService;
    private readonly IUserService _userService;
    private readonly ITableService _tableService;
    private readonly ISectionService _sectionService;

    public AppTableService(IGenericRepository<Section> sectionRepository, IGenericRepository<OrderTableMapping> orderTableRepository, IWaitingListService waitingService, IUserService userService, ITableService tableService, ISectionService sectionService)
    {
        _sectionRepository = sectionRepository;
        _orderTableRepository = orderTableRepository;
        _waitingService = waitingService;
        _userService = userService;
        _tableService = tableService;
        _sectionService = sectionService;
    }

    #region  Get

    public async Task<List<SectionViewModel>> Get()
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
                OrderAmount = t.OrderTableMappings.Where(otm => otm.TableId == t.Id && !otm.IsDeleted).Any(otm => otm.OrderId == null) ? 0 :
                            t.OrderTableMappings.Where(otm => otm.TableId == t.Id && !otm.IsDeleted).Select(otm => otm.Order!.FinalAmount).FirstOrDefault(),
                OrderTime = t.OrderTableMappings
                            .Where(otm => otm.TableId == t.Id && !otm.IsDeleted)
                            .Select(otm => otm.CreatedAt)
                            .FirstOrDefault(),
                Capacity = t.Capacity,
                CustomerId = t.OrderTableMappings
                            .Where(otm => otm.TableId == t.Id && !otm.IsDeleted)
                            .Select(otm => otm.CustomerId)
                            .FirstOrDefault(),
            }).ToList()
        }).ToList();

        return sections;
    }

    public async Task<AssignTableViewModel> Get(long tokenId)
    {
        WaitingTokenViewModel token = await _waitingService.Get(tokenId) ?? throw new NotFoundException(NotificationMessages.NotFound.Replace("{0}", "Token"));


        AssignTableViewModel assignTableVM = new();
        assignTableVM.WaitingToken.Id = tokenId;
        assignTableVM.WaitingToken.Sections = await _sectionService.Get();
        assignTableVM.WaitingToken.SectionId = token.SectionId;
        assignTableVM.WaitingToken.CustomerId = token.CustomerId;
        assignTableVM.WaitingToken.Members = token.Members;
        assignTableVM.Tables = _tableService.List(assignTableVM.WaitingToken.SectionId).Result.Where(t => t.StatusName == "Available").ToList();

        return assignTableVM;
    }

    #endregion Get

    #region  Add

    public async Task<ResponseViewModel> Add(AssignTableViewModel assignTableVM)
    {
        //Add Waiting Token
        ResponseViewModel response = await _waitingService.Save(assignTableVM.WaitingToken);

        assignTableVM.WaitingToken.Id = response.EntityId;
        assignTableVM.WaitingToken.CustomerId = _waitingService.Get(assignTableVM.WaitingToken.Id).Result.CustomerId;

        //Assign Table
        return await AssignTable(assignTableVM);
    }

    public async Task<ResponseViewModel> AssignTable(AssignTableViewModel assignTableVM)
    {
        ResponseViewModel response = new();

        if (assignTableVM.Tables.Sum(t => t.Capacity) < assignTableVM.WaitingToken.Members)
        {
            response.Success = false;
            response.Message = NotificationMessages.CapacityExceeded;
            return response;
        }

        //Assign Table
        foreach (TableViewModel? table in assignTableVM.Tables)
        {
            OrderTableMapping mapping = new()
            {
                TableId = table.Id,
                CustomerId = assignTableVM.WaitingToken.CustomerId,
                CreatedBy = await _userService.LoggedInUser()
            };

            //Change table Status from available to assigned and add mapping
            await _tableService.ChangeStatus(table.Id, SetTableStatus.ASSIGNED);
            await _orderTableRepository.AddAsync(mapping);
        }

        // Change assign status in Waiting Token
        await _waitingService.AssignTable(assignTableVM.WaitingToken.Id);
        response.Success = true;
        response.Message = NotificationMessages.Successfully.Replace("{0}", "Table Assigned");
        return response;
    }

    #endregion Add



}

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Common;
using PizzaShop.Service.Exceptions;
using PizzaShop.Service.Helpers;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Services;

public class KotService : IKotService
{
    private readonly IGenericRepository<Order> _orderRepository;
    private readonly IGenericRepository<OrderItem> _orderItemRepository;
    private readonly IGenericRepository<OrderStatus> _orderStatusRepository;

    public KotService(IGenericRepository<Order> orderRepository, IGenericRepository<OrderItem> orderItemRepository, IGenericRepository<OrderStatus> orderStatusRepository)
    {
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
        _orderStatusRepository = orderStatusRepository;
    }

    #region Get
    /*----------------------------------------------------Get Category List----------------------------------------------------------------------------------------------------------------------------------------------------
    --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    public async Task<KotViewModel> Get(long categoryId, int pageSize, int pageNumber, bool isReady)
    {

        long completeId = _orderStatusRepository.GetByStringAsync(os => os.Name == "Completed").Result!.Id;
        long cancelId = _orderStatusRepository.GetByStringAsync(os => os.Name == "Cancelled").Result!.Id;

        IEnumerable<Order> orders = await _orderRepository.GetByCondition(
        predicate: o => !o.IsDeleted
                        && o.StatusId != completeId
                        && o.StatusId != cancelId
                        && o.OrderItems.Any(oi => !oi.IsDeleted
                        && (categoryId == 0 || oi.Item.CategoryId == categoryId)),
        orderBy: q => q.OrderBy(o => o.Id),
        thenIncludes: new List<Func<IQueryable<Order>, IQueryable<Order>>>
        {
                q => q.Include(o => o.OrderTableMappings)
                    .ThenInclude(otm => otm.Table)
                    .ThenInclude(t => t.Section),
                q => q.Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Item)
                    .ThenInclude(i => i.Category),
                q => q.Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.OrderItemsModifiers)
                    .ThenInclude(m => m.Modifier),
        });

        KotViewModel kot = new()
        {
            CategoryId = categoryId,
            CategoryName = categoryId == 0 ? "All" : orders.SelectMany(o => o.OrderItems)
                                                            .Where(oi => oi.Item.CategoryId == categoryId)
                                                            .Select(oi => oi.Item.Category.Name)
                                                            .FirstOrDefault()!,
            IsReady = isReady,
            KotCards = orders.Select(o => new KotCardViewModel
            {
                OrderId = o.Id,

                SectionName = o.OrderTableMappings
                            .Where(otm => otm.OrderId == o.Id)
                            .Select(otm => otm.Table.Section.Name)
                            .FirstOrDefault()!,

                Tables = o.OrderTableMappings
                            .Where(otm => otm.OrderId == o.Id)
                            .Select(otm => otm.Table.Name)
                            .ToList(),

                Time = o.CreatedAt,

                Items = o.OrderItems
                        .Where(oi => (categoryId == 0 || oi.Item.CategoryId == categoryId) && (!oi.IsDeleted) && ((isReady && oi.ReadyQuantity > 0) || (!isReady && oi.Quantity - oi.ReadyQuantity > 0)))
                        .Select(oi => new OrderItemViewModel
                        {
                            ItemId = oi.ItemId,
                            Name = oi.Item.Name,
                            Quantity = isReady ? oi.ReadyQuantity : oi.Quantity - oi.ReadyQuantity,
                            ModifiersList = oi.OrderItemsModifiers
                                            .Select(oim => new ModifierViewModel
                                            {
                                                Name = oim.Modifier.Name,
                                                Quantity = oim.Quantity
                                            }).ToList(),
                            Instruction = oi.Instructions,
                        }).ToList(),

                Instruction = o.Instructions

            }).Where(c => c.Items.Count > 0).ToList(),
            
            Page = new()
        };

        return kot;
    }

    #endregion

    #region Update

    public async Task<ResponseViewModel> Update(KotCardViewModel kot)
    {
        ResponseViewModel response = new();

        if (kot.Items.All(i => i.IsSelected == false))
        {
            response.Success = false;
            response.Message = NotificationMessages.AtleastOne.Replace("{0}", "Item");
            return response;
        }

        foreach (OrderItemViewModel? item in kot.Items)
        {
            if (item.IsSelected)
            {
                OrderItem? orderItem = await _orderItemRepository.GetByStringAsync(
                                        oi => oi.OrderId == kot.OrderId 
                                        && oi.ItemId == item.ItemId 
                                        && !oi.IsDeleted)
                                        ?? throw new NotFoundException(NotificationMessages.NotFound.Replace("{0}", "Item"));

                orderItem.ReadyQuantity = kot.IsReady ?
                                          orderItem.ReadyQuantity - item.Quantity
                                        : orderItem.ReadyQuantity + item.Quantity;

                await _orderItemRepository.UpdateAsync(orderItem);
            }
        }

        response.Success = true;
        response.Message = NotificationMessages.Updated.Replace("{0}", "Item Status");
        return response;
    }

    #endregion

}

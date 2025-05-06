using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Common;
using PizzaShop.Service.Helpers;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Services;

public class KotService : IKotService
{

    private readonly IGenericRepository<Order> _orderRepository;
    private readonly IGenericRepository<OrderItem> _orderItemRepository;


    public KotService(IGenericRepository<Order> orderRepository, IGenericRepository<OrderItem> orderItemRepository)
    {
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
    }

    #region Get
    /*----------------------------------------------------Get Category List----------------------------------------------------------------------------------------------------------------------------------------------------
    --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    public async Task<KotViewModel> Get(long categoryId, int pageSize, int pageNumber, bool isReady)
    {
        try
        {
            IEnumerable<Order> orders = await _orderRepository.GetByCondition(
            predicate: o => !o.IsDeleted
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
            }
        );

            // (orders, int totalRecord) = await _orderRepository.GetPagedRecords(pageSize, pageNumber, orders);

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
                            .Where(oi => (categoryId == 0 || oi.Item.CategoryId == categoryId) && ((isReady && oi.ReadyQuantity > 0) || (!isReady && oi.Quantity - oi.ReadyQuantity > 0)))
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

            // kot.Page.SetPagination(totalRecord, pageSize, pageNumber);

            return kot;
        }
        catch (Exception ex)
        {
            return null;
        }

    }

    #endregion

    #region Update

    public async Task<ResponseViewModel> Update(KotCardViewModel kot)
    {
        try
        {
            if (kot.Items.All(i => i.IsSelected == false))
            {
                return new ResponseViewModel
                {
                    Success = false,
                    Message = NotificationMessages.AtleastOne.Replace("{0}", "Item")
                };
            }

            foreach (OrderItemViewModel? item in kot.Items)
            {
                if (item.IsSelected)
                {
                    OrderItem? orderItem = await _orderItemRepository.GetByStringAsync(oi => oi.OrderId == kot.OrderId && oi.ItemId == item.ItemId && !oi.IsDeleted);
                    if (orderItem == null)
                    {
                        return new ResponseViewModel
                        {
                            Success = false,
                            Message = NotificationMessages.NotFound.Replace("{0}", "Item")
                        };
                    }

                    if (kot.IsReady)
                    {
                        orderItem.ReadyQuantity -= item.Quantity;
                    }
                    else
                    {
                        orderItem.ReadyQuantity += item.Quantity;
                    }

                    if (!await _orderItemRepository.UpdateAsync(orderItem))
                    {
                        return new ResponseViewModel
                        {
                            Success = false,
                            Message = NotificationMessages.UpdatedFailed.Replace("{0}", "Item Status")
                        };
                    }
                }
            }

            return new ResponseViewModel
            {
                Success = true,
                Message = NotificationMessages.Updated.Replace("{0}", "Item Status")
            };
        }
        catch (Exception ex)
        {
            return new ResponseViewModel{
                Success = false,
                Message = NotificationMessages.UpdatedFailed.Replace("{0}", "Item Status"),
                ExMessage = ex.Message
            };
        }


    }

    #endregion



}

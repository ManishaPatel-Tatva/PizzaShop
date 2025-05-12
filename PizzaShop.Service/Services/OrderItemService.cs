using System.Linq.Expressions;
using System.Threading.Tasks;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Common;
using PizzaShop.Service.Exceptions;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Services;

public class OrderItemService : IOrderItemService
{
    private readonly IGenericRepository<OrderItem> _orderItemRepository;
    private readonly IUserService _userService;
    private readonly IOrderItemModifierService _oimService;

    public OrderItemService(IGenericRepository<OrderItem> orderItemRepository, IUserService userService, IOrderItemModifierService oimService)
    {
        _orderItemRepository = orderItemRepository;
        _userService = userService;
        _oimService = oimService;
    }

    #region Save
    public async Task Save(OrderItemViewModel orderItemVM, long orderId)
    {
        OrderItem orderItem = _orderItemRepository.GetByCondition(
                            predicate: oi => oi.Id == orderItemVM.Id && oi.OrderId == orderId && !oi.IsDeleted
                        ).Result.FirstOrDefault()
                        ?? new OrderItem
                        {
                            OrderId = orderId,
                            ItemId = orderItemVM.ItemId,
                            CreatedBy = await _userService.LoggedInUser()
                        };

        orderItem.Quantity = orderItemVM.Quantity;
        orderItem.Price = orderItemVM.Price;
        orderItem.Instructions = orderItemVM.Instruction;
        orderItem.UpdatedAt = DateTime.Now;
        orderItem.UpdatedBy = await _userService.LoggedInUser();

        if (orderItem.Id == 0)
        {
            orderItem.Id = await _orderItemRepository.AddAsyncReturnId(orderItem);
            await _oimService.Add(orderItemVM.ModifiersList, orderItem.Id);
        }
        else
        {
            await _orderItemRepository.UpdateAsync(orderItem);
        }
    }

    public async Task Save(List<OrderItemViewModel> items, long orderId)
    {
        List<long>? existingItems = _orderItemRepository.GetByCondition(
                predicate: oi => oi.OrderId == orderId && !oi.IsDeleted
            ).Result
            .Select(oi => oi.Id = oi.Id)
            .ToList();

        List<long>? removeItems = existingItems.Except(items.Select(oi => oi.Id)).ToList();

        foreach (long orderItemId in removeItems)
        {
            await Delete(orderItemId);
        }

        foreach (OrderItemViewModel? item in items)
        {
            await Save(item, orderId);
        }
    }
    #endregion

    #region Delete
    public async Task Delete(long orderItemId)
    {
        OrderItem removeItem = await _orderItemRepository.GetByIdAsync(orderItemId)
                                    ?? throw new NotFoundException(NotificationMessages.NotFound.Replace("{0}", "Item")); ;

        removeItem.IsDeleted = true;
        removeItem.UpdatedBy = await _userService.LoggedInUser();
        removeItem.UpdatedAt = DateTime.Now;

        await _orderItemRepository.UpdateAsync(removeItem);
    }
    #endregion Delete

    #region Common
    public async Task<decimal> OrderItemTotal(long orderId)
    {
        IEnumerable<OrderItem>? list = await _orderItemRepository.GetByCondition(
            oi => oi.OrderId == orderId && !oi.IsDeleted,
            includes: new List<Expression<Func<OrderItem, object>>>
            {
                oi => oi.OrderItemsModifiers
            }
        );

        decimal total = list.Sum(oi => (oi.Price * oi.Quantity) + oi.OrderItemsModifiers.Sum(oim => oim.Price * oi.Quantity));
        return total;
    }
    #endregion

}

using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
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

    public async Task<bool> Save(List<OrderItemViewModel> items, long orderId)
    {
        List<long>? existingItems = _orderItemRepository.GetByCondition(
                predicate: oi => oi.OrderId == orderId && !oi.IsDeleted
            ).Result
            .Select(oi => oi.Id = oi.Id)
            .ToList();

        List<long>? removeItems = existingItems.Except(items.Select(oi => oi.Id)).ToList();

        foreach (long orderItemId in removeItems)
        {
            OrderItem? removeItem = await _orderItemRepository.GetByIdAsync(orderItemId);
            if (removeItem == null)
            {
                return false;
            }

            removeItem.IsDeleted = true;
            removeItem.UpdatedBy = await _userService.LoggedInUser();
            removeItem.UpdatedAt = DateTime.Now;

            if (!await _orderItemRepository.UpdateAsync(removeItem))
            {
                return false;
            }
        }

        foreach (OrderItemViewModel? item in items)
        {
            OrderItem? orderItem = await _orderItemRepository.GetByIdAsync(item.Id);

            orderItem ??= new OrderItem
            {
                OrderId = orderId,
                ItemId = item.ItemId,
                CreatedBy = await _userService.LoggedInUser()
            };

            orderItem.Quantity = item.Quantity;
            orderItem.Price = item.Price;
            orderItem.Instructions = item.Instruction;
            orderItem.UpdatedAt = DateTime.Now;
            orderItem.UpdatedBy = await _userService.LoggedInUser();

            if (orderItem.Id == 0)
            {
                orderItem.Id = await _orderItemRepository.AddAsyncReturnId(orderItem);

                if (orderItem.Id < 1)
                {
                    return false;
                }
                else
                {
                    if (!await _oimService.Add(item.ModifiersList, orderItem.Id))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!await _orderItemRepository.UpdateAsync(orderItem))
                {
                    return false;
                }
            }
        }

        return true;
    }

    public decimal OrderItemTotal(long orderId)
    {
        return _orderItemRepository.GetByCondition(
            oi => oi.OrderId == orderId && !oi.IsDeleted
        ).Result!.Sum(oi => oi.Price);
    }


}

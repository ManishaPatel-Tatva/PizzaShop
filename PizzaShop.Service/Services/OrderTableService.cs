using PizzaShop.Entity.Models;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Services;

public class OrderTableService : IOrderTableService
{
    private readonly IGenericRepository<OrderTableMapping> _orderTableRepository;
    private readonly IGenericRepository<Order> _orderRepository;
    private readonly ITableService _tableService;
    private readonly IUserService _userService;

    public OrderTableService(IGenericRepository<OrderTableMapping> orderTableRepository, IUserService userService, IGenericRepository<Order> orderRepository, ITableService tableService)
    {
        _orderTableRepository = orderTableRepository;
        _userService = userService;
        _orderRepository = orderRepository;
        _tableService = tableService;
    }

    public async Task<bool> Update(long orderId)
    {
        long customerId = _orderRepository.GetByIdAsync(orderId).Result!.CustomerId;
        IEnumerable<OrderTableMapping>? mappings = await _orderTableRepository.GetByCondition(ot => ot.CustomerId == customerId && !ot.IsDeleted);
        foreach (OrderTableMapping? mapping in mappings)
        {
            mapping.OrderId = orderId;
            mapping.UpdatedBy = await _userService.LoggedInUser();
            mapping.UpdatedAt = DateTime.Now;

            if (await _orderTableRepository.UpdateAsync(mapping))
            {
                await _tableService.SetTableOccupied(mapping.TableId);
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    public async Task<bool> Delete(long orderId)
    {
        IEnumerable<OrderTableMapping>? mappings = await _orderTableRepository.GetByCondition(ot => ot.OrderId == orderId && !ot.IsDeleted);

        bool success = false;

        foreach (OrderTableMapping mapping in mappings)
        {
            mapping.IsDeleted = true;
            mapping.UpdatedBy = await _userService.LoggedInUser();
            mapping.UpdatedAt = DateTime.Now;

            success = await _orderTableRepository.UpdateAsync(mapping);
            if (!success)
            {
                return success;
            }

            //Change table status to available
            success = await _tableService.SetTableAvailable(mapping.TableId);
            if (!success)
            {
                return success;
            }
        }
        
        return true;
    }

}

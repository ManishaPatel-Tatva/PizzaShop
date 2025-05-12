using PizzaShop.Entity.Models;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Common;
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

    public async Task Update(long orderId)
    {
        long customerId = _orderRepository.GetByIdAsync(orderId).Result!.CustomerId;
        IEnumerable<OrderTableMapping>? mappings = await _orderTableRepository.GetByCondition(ot => ot.CustomerId == customerId && !ot.IsDeleted);
        foreach (OrderTableMapping? mapping in mappings)
        {
            mapping.OrderId = orderId;
            mapping.UpdatedBy = await _userService.LoggedInUser();
            mapping.UpdatedAt = DateTime.Now;

            await _orderTableRepository.UpdateAsync(mapping);
            await _tableService.ChangeStatus(mapping.TableId, SetTableStatus.OCCUPIED);
        }
    }

    public async Task Delete(long orderId)
    {
        IEnumerable<OrderTableMapping>? mappings = await _orderTableRepository.GetByCondition(ot => ot.OrderId == orderId && !ot.IsDeleted);

        foreach (OrderTableMapping mapping in mappings)
        {
            mapping.IsDeleted = true;
            mapping.UpdatedBy = await _userService.LoggedInUser();
            mapping.UpdatedAt = DateTime.Now;

            await _orderTableRepository.UpdateAsync(mapping);

            //Change table status to available
            await _tableService.ChangeStatus(mapping.TableId, SetTableStatus.AVAILABLE);
        }
    }

}

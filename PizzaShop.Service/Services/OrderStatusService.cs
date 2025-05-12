using PizzaShop.Entity.Models;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Common;
using PizzaShop.Service.Exceptions;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Services;

public class OrderStatusService : IOrderStatusService
{
    private readonly IGenericRepository<OrderStatus> _orderStatusRepository;

    public OrderStatusService(IGenericRepository<OrderStatus> orderStatusRepository)
    {
        _orderStatusRepository = orderStatusRepository;
    }

    public async Task<long> Get(string status)
    {
        OrderStatus orderStatus = await _orderStatusRepository.GetByStringAsync(os => os.Name == status)
        ?? throw new NotFoundException(NotificationMessages.NotFound.Replace("{0}", "Order Status"));
        
        return orderStatus.Id;
    }


}

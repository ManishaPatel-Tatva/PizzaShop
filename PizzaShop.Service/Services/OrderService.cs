using System.Linq.Expressions;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Helpers;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Services;

public class OrderService : IOrderService
{
    private readonly IGenericRepository<Order> _orderRepository;
    private readonly IGenericRepository<OrderStatus> _orderStatusRepository;

    public OrderService(IGenericRepository<Order> orderRepository, IGenericRepository<OrderStatus> orderStatusRepository)
    {
        _orderRepository = orderRepository;
        _orderStatusRepository = orderStatusRepository;

    }

    public async Task<OrderIndexViewModel> GetOrderIndex()
    {
        OrderIndexViewModel model = new()
        {
            Statuses = _orderStatusRepository.GetAll().ToList()
        };
        return model;
    }

    public async Task<OrderPaginationViewModel> GetPagedOrder(int pageSize, int pageNumber, string search)
    {
        (IEnumerable<Order> orders, int totalRecord) = await _orderRepository.GetPagedRecordsAsync(
            pageSize,
            pageNumber,
            filter: o => !o.IsDeleted &&
                    (string.IsNullOrEmpty(search.ToLower()) ||
                    o.Customer.Name.ToLower().Contains(search.ToLower())),
            orderBy: q => q.OrderBy(u => u.Id),
            includes: new List<Expression<Func<Order, object>>>
            {
                o => o.Customer,
                o => o.PaymentMethod,
                o => o.Status,
                o => o.CustomersReviews
            }
        );

        OrderPaginationViewModel model = new() 
        { 
            Page = new(),
            Orders = orders.Select(o => new OrderViewModel(){
                OrderId = o.Id,
                Date = o.Date,
                CustomerName = o.Customer.Name,
                Status = o.Status.Name,
                PaymentMode = o.PaymentMethod.Name,
                Rating = (int)(o.CustomersReviews.Any() ? o.CustomersReviews.Average(r => r.Rating) : 0),
                TotalAmount = o.TotalAmount
            })
        };

        model.Page.SetPagination(totalRecord, pageSize, pageNumber);
        return model;
    }

}

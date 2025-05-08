using Microsoft.EntityFrameworkCore;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Services;

public class DashboardService : IDashboardService
{
    private readonly IGenericRepository<Order> _orderRepository;
    private readonly IGenericRepository<WaitingToken> _waitingTokenRepository;
    private readonly IGenericRepository<Customer> _customerRepository;

    public DashboardService(IGenericRepository<Order> orderRepository, IGenericRepository<WaitingToken> waitingTokenRepository, IGenericRepository<Customer> customerRepository)
    {
        _orderRepository = orderRepository;
        _waitingTokenRepository = waitingTokenRepository;
        _customerRepository = customerRepository;
    }

    public async Task<DashboardViewModel> Get(FilterViewModel filter)
    {
        Func<DateTime, bool> dateFilter = t => true;

        // Time Range Filter
        if (!string.IsNullOrEmpty(filter.DateRange) && !filter.FromDate.HasValue && !filter.ToDate.HasValue)
        {
            switch (filter.DateRange)
            {
                case "Today":
                    DateOnly today = DateOnly.FromDateTime(DateTime.Now);
                    dateFilter = t => t.Day == today.Day && t.Month == today.Month && t.Year == today.Year;
                    break;

                case "Last 7 days":
                    dateFilter = t => DateOnly.FromDateTime(t) >= DateOnly.FromDateTime(DateTime.Now.AddDays(-7)) && DateOnly.FromDateTime(t) <= DateOnly.FromDateTime(DateTime.Now);
                    break;

                case "Last 30 days":
                    dateFilter = t => DateOnly.FromDateTime(t) >= DateOnly.FromDateTime(DateTime.Now.AddDays(-30)) && DateOnly.FromDateTime(t) <= DateOnly.FromDateTime(DateTime.Now);
                    break;

                case "Current Month":
                    DateOnly currentDay = DateOnly.FromDateTime(DateTime.Now);
                    dateFilter = t => t.Month == currentDay.Month && t.Year == currentDay.Year;
                    break;

                default:
                    break;
            }
        }

        // Date Filter
        if (filter.FromDate.HasValue)
        {
            dateFilter = t => DateOnly.FromDateTime(t) >= filter.FromDate.Value;
        }

        if (filter.ToDate.HasValue)
        {
            dateFilter = t => DateOnly.FromDateTime(t) <= filter.ToDate.Value;
        }

        if (filter.FromDate.HasValue && filter.ToDate.HasValue)
        {
            dateFilter = t => DateOnly.FromDateTime(t) >= filter.FromDate.Value && DateOnly.FromDateTime(t) <= filter.ToDate.Value;
        }

        List<Order> orderList = _orderRepository.GetByCondition(
            thenIncludes: new List<Func<IQueryable<Order>, IQueryable<Order>>>
            {
                q => q.Include(o => o.OrderItems)
                .ThenInclude(oi =>oi.Item)
            }
        ).Result
        .Where(o => dateFilter(o.CreatedAt)).ToList();

        List<WaitingToken> waitingList = _waitingTokenRepository.GetAll().Where(w => dateFilter(w.CreatedAt)).ToList();
        
        List<Customer> customerList = _customerRepository.GetAll().Where(c => dateFilter(c.CreatedAt)).ToList();

        List<SellingItem> orderItemList = orderList.SelectMany(o => o.OrderItems)
                        .Where(oi => !oi.IsDeleted)
                        .GroupBy(oi => new { oi.Item.Name, oi.Item.ImageUrl })
                        .Select(g => new SellingItem
                        {
                            Name = g.Key.Name,
                            ImgUrl = g.Key.ImageUrl,
                            TotalQuantity = g.Sum(oi => oi.Quantity)
                        }).ToList();

        List<RevenuData> revenuDataList = orderList.GroupBy(o => o.CreatedAt.Day)
                                        .Select(g => new RevenuData
                                        {
                                            Date = g.Key,
                                            Revenue = g.Sum(o => o.FinalAmount)
                                        }).ToList();

        List<CustomerData> customerDataList = customerList.GroupBy(c => c.CreatedAt.Month)
                                        .Select(g => new CustomerData
                                        {
                                            Month = g.Key,
                                            CustomerCount = g.Count()
                                        }).ToList();

        DashboardViewModel dashboard = new()
        {
            TotalOrders = orderList.Any() ? orderList.Count() : 0,
            TotalSales = orderList.Any() ? orderList.Sum(o => o.FinalAmount) : 0,
            AvgOrderValue = orderList.Any() ? orderList.Average(o => o.SubTotal) : 0,
            WaitingListCount = waitingList.Any() ? waitingList.Where(w => !w.IsAssigned).Count() : 0,
            AvgWaitingTime = waitingList.Any(w => w.IsAssigned) ? waitingList.Where(w => w.IsAssigned).Average(w => (w.AssignedAt - w.CreatedAt).Value.TotalMinutes) : 0,
            NewCustomerCount = customerList.Any() ? customerList.Count(c => dateFilter(c.CreatedAt)) : 0,
            TopSellingItems = orderList.Any() ? orderItemList.OrderByDescending(oi => oi.TotalQuantity).Take(2).ToList() : new(),
            LeastSellingItems = orderList.Any() ? orderItemList.OrderBy(oi => oi.TotalQuantity).Take(2).ToList() : new()
        };

        for (int i = 1; i <= 31; i++)
        {
            dashboard.Revenue.Add(revenuDataList.FirstOrDefault(r => r.Date == i)?.Revenue ?? 0);
        }

        for (int i = 1; i <= 12; i++)
        {
            dashboard.CustomerCount.Add(customerDataList.FirstOrDefault(c => c.Month == i)?.CustomerCount ?? 0);
        }

        return dashboard;
    }



}

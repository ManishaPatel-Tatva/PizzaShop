using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
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
    #region Order Pagination
    /*----------------------------------------------------Order Pagination----------------------------------------------------------------------------------------------------------------------------------------------------
    --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<OrderPaginationViewModel> GetPagedRecord(FilterViewModel filter)
    {
        filter.Search = string.IsNullOrEmpty(filter.Search) ? "" : filter.Search;

        (IEnumerable<Order> orders, int totalRecord) = await _orderRepository.GetPagedRecordsAsync(
            filter.PageSize,
            filter.PageNumber,
            predicate: o => !o.IsDeleted &&
                    (string.IsNullOrEmpty(filter.Search.ToLower()) ||
                    o.Customer.Name.ToLower().Contains(filter.Search.ToLower())),
            orderBy: q => q.OrderBy(u => u.Id),
            includes: new List<Expression<Func<Order, object>>>
            {
            o => o.Customer,
            o => o.Payments,
            o => o.Status,
            o => o.CustomersReviews,
            o => o.Invoices
            },
            thenIncludes: new List<Func<IQueryable<Order>, IQueryable<Order>>>
            {
            q => q.Include(op => op.Payments)
            .ThenInclude(p => p.PaymentMethod)
            }
        );

        //For applying status filter
        if (!string.IsNullOrEmpty(filter.Status) && filter.Status.ToLower() != "all status")
        {
            orders = orders.Where(o => o.Status.Name.ToLower() == filter.Status.ToLower());
        }

        //For applying date range filter
        if (!string.IsNullOrEmpty(filter.DateRange) && filter.DateRange.ToLower() != "all time" && !filter.FromDate.HasValue && !filter.ToDate.HasValue)
        {
            switch (filter.DateRange.ToLower())
            {
                case "last 7 days":
                    orders = orders.Where(o => DateOnly.FromDateTime(o.CreatedAt) >= DateOnly.FromDateTime(DateTime.Now.AddDays(-7)) && DateOnly.FromDateTime(o.CreatedAt) <= DateOnly.FromDateTime(DateTime.Now));
                    break;
                case "last 30 days":
                    orders = orders.Where(o => DateOnly.FromDateTime(o.CreatedAt) >= DateOnly.FromDateTime(DateTime.Now.AddDays(-30)) && DateOnly.FromDateTime(o.CreatedAt) <= DateOnly.FromDateTime(DateTime.Now));
                    break;
                case "current month":
                    DateOnly startDate = DateOnly.FromDateTime(DateTime.Now);
                    orders = orders.Where(o => DateOnly.FromDateTime(o.CreatedAt).Month == startDate.Month && DateOnly.FromDateTime(o.CreatedAt).Year == startDate.Year);
                    break;
                default:
                    break;
            }
        }

        //Filtering Custom Dates
        if (filter.FromDate.HasValue)
            orders = orders.Where(o => DateOnly.FromDateTime(o.CreatedAt) >= filter.FromDate.Value);
        if (filter.ToDate.HasValue)
            orders = orders.Where(o => DateOnly.FromDateTime(o.CreatedAt) <= filter.ToDate.Value);

        //For sorting the column according to order
        if (!string.IsNullOrEmpty(filter.Column))
        {
            switch (filter.Column)
            {
                case "order":
                    orders = filter.Sort == "asc" ? orders.OrderBy(o => o.Id) : orders.OrderByDescending(o => o.Id);
                    break;
                case "date":
                    orders = filter.Sort == "asc" ? orders.OrderBy(o => DateOnly.FromDateTime(o.CreatedAt)) : orders.OrderByDescending(o => DateOnly.FromDateTime(o.CreatedAt));
                    break;
                case "customer":
                    orders = filter.Sort == "asc" ? orders.OrderBy(o => o.Customer.Name) : orders.OrderByDescending(o => o.Customer.Name);
                    break;
                case "amount":
                    orders = filter.Sort == "asc" ? orders.OrderBy(o => o.FinalAmount) : orders.OrderByDescending(o => o.FinalAmount);
                    break;
                default:
                    break;
            }
        }

        //Setting the filtered and sorted values in View Model
        OrderPaginationViewModel model = new()
        {
            Page = new(),
            Orders = orders.Select(o => new OrderViewModel()
            {
                OrderId = o.Id,
                Date = DateOnly.FromDateTime(o.CreatedAt),
                CustomerName = o.Customer.Name,
                Status = o.Status.Name,
                PaymentMode = o.Payments.Where(p => p.OrderId == o.Id).Select(p => p.PaymentMethod.Name).First(),
                Rating = (int)(o.CustomersReviews.Any() ? o.CustomersReviews.Average(r => r.Rating) : 0),
                TotalAmount = o.FinalAmount
            })
        };

        totalRecord = model.Orders.Count();

        model.Page.SetPagination(totalRecord, filter.PageSize, filter.PageNumber);
        return model;
    }
    #endregion

    #region Export Excel
    /*----------------------------------------------------Export Order List----------------------------------------------------------------------------------------------------------------------------------------------------
    --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<byte[]> ExportExcel(string status, string dateRange, DateOnly? fromDate, DateOnly? toDate, string column, string sort, string search)
    {
        IEnumerable<Order> orders = await _orderRepository.GetByCondition(
            predicate: o => !o.IsDeleted &&
                    (string.IsNullOrEmpty(search.ToLower()) ||
                    o.Customer.Name.ToLower().Contains(search.ToLower())),
            orderBy: q => q.OrderBy(u => u.Id),
            includes: new List<Expression<Func<Order, object>>>
            {
                o => o.Customer,
                o => o.Payments,
                o => o.Status,
                o => o.CustomersReviews
            },
            thenIncludes: new List<Func<IQueryable<Order>, IQueryable<Order>>>
            {
                q => q.Include(o => o.Payments)
                    .ThenInclude(p => p.PaymentMethod)
            }
        );

        //For applying status filter
        if (!string.IsNullOrEmpty(status) && status.ToLower() != "all status")
        {
            orders = orders.Where(o => o.Status.Name.ToLower() == status.ToLower());
        }

        //For applying date range filter
        if (!string.IsNullOrEmpty(dateRange) && dateRange.ToLower() != "all time" && !fromDate.HasValue && !toDate.HasValue)
        {
            switch (dateRange.ToLower())
            {
                case "last 7 days":
                    orders = orders.Where(o => DateOnly.FromDateTime(o.CreatedAt) >= DateOnly.FromDateTime(DateTime.Now.AddDays(-7)) && DateOnly.FromDateTime(o.CreatedAt) <= DateOnly.FromDateTime(DateTime.Now));
                    break;
                case "last 30 days":
                    orders = orders.Where(o => DateOnly.FromDateTime(o.CreatedAt) >= DateOnly.FromDateTime(DateTime.Now.AddDays(-7)) && DateOnly.FromDateTime(o.CreatedAt) <= DateOnly.FromDateTime(DateTime.Now));
                    break;
                case "current month":
                    DateOnly startDate = DateOnly.FromDateTime(DateTime.Now);
                    orders = orders.Where(o => DateOnly.FromDateTime(o.CreatedAt).Month == startDate.Month && DateOnly.FromDateTime(o.CreatedAt).Year == startDate.Year);
                    break;
                default:
                    break;
            }
        }

        //Filtering Custom Dates
        if (fromDate.HasValue)
            orders = orders.Where(o => DateOnly.FromDateTime(o.CreatedAt) >= fromDate.Value);
        if (toDate.HasValue)
            orders = orders.Where(o => DateOnly.FromDateTime(o.CreatedAt) <= toDate.Value);

        //For sorting the column according to order
        if (!string.IsNullOrEmpty(column))
        {
            switch (column.ToLower())
            {
                case "order":
                    orders = sort == "asc" ? orders.OrderBy(o => o.Id) : orders.OrderByDescending(o => o.Id);
                    break;
                case "date":
                    orders = sort == "asc" ? orders.OrderBy(o => DateOnly.FromDateTime(o.CreatedAt)) : orders.OrderByDescending(o => DateOnly.FromDateTime(o.CreatedAt));
                    break;
                case "customer":
                    orders = sort == "asc" ? orders.OrderBy(o => o.Customer.Name) : orders.OrderByDescending(o => o.Customer.Name);
                    break;
                case "amount":
                    orders = sort == "asc" ? orders.OrderBy(o => o.FinalAmount) : orders.OrderByDescending(o => o.FinalAmount);
                    break;
                default:
                    break;
            }
        }

        List<OrderViewModel> orderList = orders.Select(o => new OrderViewModel()
            {
                OrderId = o.Id,
                Date = DateOnly.FromDateTime(o.CreatedAt),
                CustomerName = o.Customer.Name,
                Status = o.Status.Name,
                PaymentMode = o.Payments.Where(p => p.OrderId == o.Id).Select(p => p.PaymentMethod.Name).First(),
                Rating = (int)(o.CustomersReviews.Any() ? o.CustomersReviews.Average(r => r.Rating) : 0),
                TotalAmount = o.FinalAmount
            }).ToList();

        return ExcelTemplateHelper.Orders(orderList, status, dateRange, search);
    }
    #endregion

    #region Order Details
    /*----------------------------------------------------Order Details----------------------------------------------------------------------------------------------------------------------------------------------------
    --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<OrderDetailViewModel> GetOrderDetail(long orderId)
    {
        try
        {

            IEnumerable<Order>? orderDetail = _orderRepository.GetByCondition(
                predicate: o => o.Id == orderId && !o.IsDeleted,
                includes: new List<Expression<Func<Order, object>>>
                {
                o => o.Status,
                o => o.Invoices,
                o => o.Customer,
                o => o.OrderTableMappings,
                o => o.OrderTaxMappings,
                o => o.OrderItems,
                o => o.Payments
                },
                thenIncludes: new List<Func<IQueryable<Order>, IQueryable<Order>>>
                {
                q => q.Include(o => o.OrderTableMappings)
                    .ThenInclude(otm => otm.Table)
                    .ThenInclude(t => t.Section),
                q => q.Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Item),
                q => q.Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.OrderItemsModifiers)
                    .ThenInclude(m => m.Modifier),
                q => q.Include(o => o.Payments)
                    .ThenInclude(p => p.PaymentMethod),
                q => q.Include(o => o.OrderTaxMappings)
                    .ThenInclude(otm => otm.Tax)
                }
            ).Result;

            var model = orderDetail
            .Select(o => new OrderDetailViewModel
            {
                OrderId = o.Id,

                OrderStatus = o.Status.Name,

                InvoiceNo = o.Invoices
                            .Where(i => i.OrderId == o.Id)
                            .Select(i => i.InvoiceNo)
                            .First(),

                PaidOn = o.Payments
                        .Where(p => p.OrderId == o.Id)
                        .Select(p => p.Date)
                        .First()
                        .ToString() ?? "",

                PlacedOn = o.CreatedAt.ToString(),

                ModifiedOn = o.UpdatedAt.ToString() ?? "",

                OrderDuration = (o.Payments.Where(p => p.OrderId == o.Id).Select(p => p.Date).First()
                                - o.CreatedAt)
                                .ToString() ?? "",

                CustomerName = o.Customer.Name,

                CustomerPhone = o.Customer.Phone,

                NoOfPerson = o.Members,

                CustomerEmail = o.Customer.Email,

                TableList = o.OrderTableMappings
                            .Where(ot => ot.OrderId == o.Id)
                            .Select(ot => ot.Table.Name)
                            .ToList(),

                Section = o.OrderTableMappings
                        .Where(ot => ot.OrderId == o.Id)
                        .Select(ot => ot.Table.Section.Name)
                        .First(),

                ItemsList = o.OrderItems
                            .Where(oi => oi.OrderId == o.Id)
                            .Select(oi => new OrderItemViewModel
                            {
                                ItemName = oi.Item.Name,
                                Quantity = oi.Quantity,
                                Price = oi.Price,
                                TotalAmount = oi.Quantity * oi.Price,
                                ModifiersList = oi.OrderItemsModifiers
                                                .Where(oim => oim.OrderItemId == oi.Id)
                                                .Select(oim => new ModifierViewModel
                                                {
                                                    ModifierName = oim.Modifier.Name,
                                                    Quantity = oim.Quantity,
                                                    Rate = oim.Price,
                                                    TotalAmount = oim.Quantity * oim.Price
                                                }).ToList()
                            }).ToList(),

                Subtotal = o.SubTotal,

                TaxList = o.OrderTaxMappings.Where(otm => otm.OrderId == o.Id)
                            .Select(otm => new TaxViewModel
                            {
                                Name = otm.Tax.Name,
                                TaxValue = otm.TaxValue
                            }).ToList(),

                FinalAmount = o.FinalAmount ,

                PaymentMethod = o.Payments.Where(p => p.OrderId == o.Id).Select(p => p.PaymentMethod.Name).First(),

            }).FirstOrDefault();


            return model;

        }
        catch (Exception ex)
        {
            return null;
        }
    }
    #endregion


}
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

public class OrderService : IOrderService
{
    private readonly IGenericRepository<Order> _orderRepository;
    private readonly IGenericRepository<OrderStatus> _orderStatusRepository;
    private readonly IOrderStatusService _orderStatusService;
    private readonly ITransactionRepository _transaction;
    private readonly IUserService _userService;
    private readonly IGenericRepository<WaitingToken> _waitingTokenRepository;
    private readonly IWaitingListService _waitingService;
    private readonly IInvoiceService _invoiceService;
    private readonly IPaymentService _paymentService;
    private readonly IOrderTableService _orderTableService;
    private readonly IOrderTaxService _orderTaxService;
    private readonly IOrderItemService _orderItemService;

    public OrderService(IGenericRepository<Order> orderRepository, IGenericRepository<OrderStatus> orderStatusRepository, IOrderStatusService orderStatusService, ITransactionRepository transaction, IUserService userService, IGenericRepository<WaitingToken> waitingTokenRepository, IWaitingListService waitingService, IInvoiceService invoiceService, IPaymentService paymentService, IOrderTableService orderTableService, IOrderTaxService orderTaxService, IOrderItemService orderItemService)
    {
        _orderRepository = orderRepository;
        _orderStatusRepository = orderStatusRepository;
        _orderStatusService = orderStatusService;
        _transaction = transaction;
        _userService = userService;
        _waitingTokenRepository = waitingTokenRepository;
        _waitingService = waitingService;
        _invoiceService = invoiceService;
        _paymentService = paymentService;
        _orderTableService = orderTableService;
        _orderTaxService = orderTaxService;
        _orderItemService = orderItemService;
    }


    #region Get
    /*----------------------------------------------------Get Order Status----------------------------------------------------------------------------------------------------------------------------------------------------
    --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public OrderIndexViewModel Get()
    {
        OrderIndexViewModel model = new()
        {
            Statuses = _orderStatusRepository.GetAll().ToList()
        };
        return model;
    }

    /*----------------------------------------------------Order Pagination----------------------------------------------------------------------------------------------------------------------------------------------------
    --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<OrderPaginationViewModel> Get(FilterViewModel filter)
    {
        IEnumerable<Order> orders = await List(filter);

        (orders, int totalRecord) = _orderRepository.GetPagedRecords(filter.PageSize, filter.PageNumber, orders);

        //Setting the filtered and sorted values in View Model
        OrderPaginationViewModel model = new()
        {
            Orders = List(orders)
        };

        model.Page.SetPagination(totalRecord, filter.PageSize, filter.PageNumber);
        return model;
    }

    #endregion

    #region List

    /*----------------------------------------------------Order List----------------------------------------------------------------------------------------------------------------------------------------------------
    --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<IEnumerable<Order>> List(FilterViewModel filter)
    {
        filter.Search = string.IsNullOrEmpty(filter.Search) ? "" : filter.Search;

        //For sorting the column according to order
        Func<IQueryable<Order>, IOrderedQueryable<Order>>? orderBy = q => q.OrderBy(o => o.Id);
        if (!string.IsNullOrEmpty(filter.Column))
        {
            switch (filter.Column)
            {
                case "order":
                    orderBy = filter.Sort == "asc" ? q => q.OrderBy(o => o.Id) : q => q.OrderByDescending(o => o.Id);
                    break;
                case "date":
                    orderBy = filter.Sort == "asc" ? q => q.OrderBy(o => DateOnly.FromDateTime(o.CreatedAt)) : q => q.OrderByDescending(o => DateOnly.FromDateTime(o.CreatedAt));
                    break;
                case "customer":
                    orderBy = filter.Sort == "asc" ? q => q.OrderBy(o => o.Customer.Name) : q => q.OrderByDescending(o => o.Customer.Name);
                    break;
                case "amount":
                    orderBy = filter.Sort == "asc" ? q => q.OrderBy(o => o.FinalAmount) : q => q.OrderByDescending(o => o.FinalAmount);
                    break;
                default:
                    break;
            }
        }

        IEnumerable<Order> orders = await _orderRepository.GetByCondition(
            predicate: o => !o.IsDeleted &&
                    (string.IsNullOrEmpty(filter.Search.ToLower()) ||
                    o.Customer.Name.ToLower().Contains(filter.Search.ToLower())),
            orderBy: orderBy,
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
        {
            orders = orders.Where(o => DateOnly.FromDateTime(o.CreatedAt) >= filter.FromDate.Value);
        }
        if (filter.ToDate.HasValue)
        {
            orders = orders.Where(o => DateOnly.FromDateTime(o.CreatedAt) <= filter.ToDate.Value);
        }

        return orders;
    }

    /*----------------------------------------------------Order View Model List----------------------------------------------------------------------------------------------------------------------------------------------------
  --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public static IEnumerable<OrderViewModel> List(IEnumerable<Order> orders)
    {
        IEnumerable<OrderViewModel>? orderList = orders.Select(o => new OrderViewModel()
        {
            OrderId = o.Id,
            Date = DateOnly.FromDateTime(o.CreatedAt),
            CustomerName = o.Customer.Name,
            Status = o.Status.Name,
            PaymentMode = o.Payments.Where(p => p.OrderId == o.Id).Select(p => p.PaymentMethod.Name).First(),
            Rating = (int)(o.CustomersReviews.Any() ? o.CustomersReviews.Average(r => r.Rating) ?? 0 : 0),
            TotalAmount = o.FinalAmount
        });

        return orderList;
    }

    #endregion

    #region Order Details
    /*----------------------------------------------------Order Details----------------------------------------------------------------------------------------------------------------------------------------------------
    --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<OrderDetailViewModel> Get(long orderId)
    {

        if (orderId == 0)
        {
            return new OrderDetailViewModel();
        }
        else
        {
            IEnumerable<Order>? orderDetail = await _orderRepository.GetByCondition(
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
            );

            OrderDetailViewModel? orderDetailVM = orderDetail
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

                Comment = o.Instructions,

                CustomerId = o.CustomerId,

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
                            .Where(oi => oi.OrderId == o.Id && !oi.IsDeleted)
                            .Select(oi => new OrderItemViewModel
                            {
                                Id = oi.Id,
                                ItemId = oi.ItemId,
                                Name = oi.Item.Name,
                                Quantity = oi.Quantity,
                                Price = oi.Price,
                                TotalAmount = oi.Quantity * oi.Price,
                                ReadyQuantity = oi.ReadyQuantity,
                                ModifiersList = oi.OrderItemsModifiers
                                                .Where(oim => oim.OrderItemId == oi.Id && !oim.IsDeleted)
                                                .Select(oim => new ModifierViewModel
                                                {
                                                    Id = oim.ModifierId,
                                                    Name = oim.Modifier.Name,
                                                    Quantity = oim.Quantity,
                                                    Rate = oim.Price,
                                                    TotalAmount = oi.Quantity * oim.Price
                                                }).ToList(),
                                Instruction = oi.Instructions
                            }).ToList(),

                Subtotal = o.SubTotal,

                TaxList = o.OrderTaxMappings.Where(otm => otm.OrderId == o.Id)
                            .Select(otm => new TaxViewModel
                            {
                                TaxId = otm.TaxId,
                                Name = otm.Tax.Name,
                                TaxValue = otm.TaxValue
                            }).ToList(),

                FinalAmount = o.FinalAmount,

                PaymentMethodId = o.Payments.Where(p => p.OrderId == o.Id).Select(p => p.PaymentMethodId).First(),

                PaymentMethod = o.Payments.Where(p => p.OrderId == o.Id).Select(p => p.PaymentMethod.Name).First(),

            }).FirstOrDefault() ?? throw new NotFoundException(NotificationMessages.NotFound.Replace("{0}", "Order"));


            return orderDetailVM;
        }
    }

    #endregion

    #region  Save
    public async Task<ResponseViewModel> Save(OrderDetailViewModel orderVM)
    {
        try
        {
            await _transaction.BeginTransactionAsync();

            Order order = await _orderRepository.GetByIdAsync(orderVM.OrderId) ?? new Order();

            if (orderVM.OrderId == 0)
            {
                order.CustomerId = orderVM.CustomerId;
                order.StatusId = 1;
                order.CreatedBy = await _userService.LoggedInUser();
                WaitingToken? token = await _waitingTokenRepository.GetByStringAsync(t => !t.IsDeleted && t.CustomerId == orderVM.CustomerId) ?? throw new NotFoundException(NotificationMessages.NotFound.Replace("{0}", "Waiting Token"));
                // await _waitingService.Delete(token.Id);
                order.Members = token.Members;

                order.Id = await _orderRepository.AddAsyncReturnId(order);

                //Create Invoice and Update order in mapping
                await _invoiceService.Add(order.Id);
                await _orderTableService.Update(order.Id);
            }

            order.Instructions = orderVM.Comment;

            // Order Item
            await _orderItemService.Save(orderVM.ItemsList, order.Id);
            order.SubTotal = await _orderItemService.OrderItemTotal(order.Id);
            await _orderRepository.UpdateAsync(order);

            //Tax on order item
            await _orderTaxService.Save(orderVM.Taxes, order.Id);
            decimal taxAmount = _orderTaxService.TotalTaxOnOrder(order.Id);

            // Final Amount Calculation
            order.FinalAmount = order.SubTotal + taxAmount;
            await _orderRepository.UpdateAsync(order);

            //Save Payment
            await _paymentService.Save(orderVM.PaymentMethodId, order.Id);

            await _transaction.CommitAsync();

            return new ResponseViewModel
            {
                Success = true,
                Message = orderVM.OrderId == 0 ? NotificationMessages.Added.Replace("{0}", "Order") : NotificationMessages.Updated.Replace("{0}", "Order")
            };
        }
        catch
        {
            await _transaction.RollbackAsync();
            throw;
        }
    }
    #endregion Save

    #region Complete/Cancel
    public async Task<ResponseViewModel> CompleteOrder(long orderId)
    {
        try
        {
            await _transaction.BeginTransactionAsync();
            ResponseViewModel response = new();

            // Order status - Completion
            OrderDetailViewModel orderDetail = await Get(orderId)
                                        ?? throw new NotFoundException(NotificationMessages.NotFound.Replace("{0}", "Order"));

            if (orderDetail.ItemsList.Any(oi => oi.ReadyQuantity != oi.Quantity))
            {
                response.Success = false;
                response.Message = NotificationMessages.CompleteOrderFailed;
                return response;
            }

            await ChangeStatus(orderId, SetOrderStatus.COMPLETED);

            // Delete Waiting Token and Table assignment
            WaitingToken? token = await _waitingTokenRepository.GetByStringAsync(t => !t.IsDeleted && t.CustomerId == orderDetail.CustomerId);
            if (token != null)
            {
                await _waitingService.Delete(token.Id);
            }

            await _orderTableService.Delete(orderId);

            await _transaction.CommitAsync();

            response.Success = true;
            response.Message = NotificationMessages.Successfully.Replace("{0}", "Order Completed");
            return response;
        }
        catch
        {
            await _transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<ResponseViewModel> CancelOrder(long orderId)
    {
        try
        {
            await _transaction.BeginTransactionAsync();

            ResponseViewModel response = new();

            // Order status - Cancelled
            OrderDetailViewModel orderDetail = await Get(orderId)
                                        ?? throw new NotFoundException(NotificationMessages.NotFound.Replace("{0}", "Order"));

            if (orderDetail.ItemsList.Any(oi => oi.ReadyQuantity > 0))
            {
                response.Success = false;
                response.Message = NotificationMessages.CannotCancelOrder;
                return response;
            }

            await ChangeStatus(orderId, SetOrderStatus.CANCELLED);

            // Delete Waiting Token and Table assignment
            WaitingToken? token = await _waitingTokenRepository.GetByStringAsync(t => !t.IsDeleted && t.CustomerId == orderDetail.CustomerId);
            if (token != null)
            {
                await _waitingService.Delete(token.Id);
            }

            await _orderTableService.Delete(orderId);

            await _transaction.CommitAsync();

            response.Success = true;
            response.Message = NotificationMessages.Successfully.Replace("{0}", "Order Cancelled");
            return response;
        }
        catch
        {
            await _transaction.RollbackAsync();
            throw;
        }
    }

    #endregion Complete/Cancel

    #region Common

    /*----------------------------------------------------Export Order List----------------------------------------------------------------------------------------------------------------------------------------------------
    --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<byte[]> ExportExcel(FilterViewModel filter)
    {

        IEnumerable<Order>? orders = await List(filter);

        IEnumerable<OrderViewModel>? orderList = List(orders);

        return ExcelTemplateHelper.Orders(orderList, filter.Status, filter.DateRange, filter.Search);
    }

    /*----------------------------------------------------Export Order List----------------------------------------------------------------------------------------------------------------------------------------------------
    --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task ChangeStatus(long orderId, string status)
    {
        Order order = await _orderRepository.GetByIdAsync(orderId) ?? throw new NotFoundException(NotificationMessages.NotFound.Replace("{0}", "Waiting Token"));
        order.StatusId = await _orderStatusService.Get(status);
        await _orderRepository.UpdateAsync(order);
    }

    #endregion


}

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Common;
using PizzaShop.Service.Exceptions;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Services;

public class AppMenuService : IAppMenuService
{
    private readonly IGenericRepository<Item> _itemRepository;
    private readonly IGenericRepository<OrderTableMapping> _orderTableRepository;
    private readonly ICategoryService _categoryService;
    private readonly IOrderService _orderService;
    private readonly IGenericRepository<Taxis> _taxRepository;
    private readonly IGenericRepository<PaymentMethod> _paymentMethodRepository;
    private readonly IUserService _userService;
    private readonly IGenericRepository<Order> _orderRepository;
    private readonly IGenericRepository<WaitingToken> _waitingTokenRepository;
    private readonly IGenericRepository<OrderStatus> _orderStatusRepository;
    private readonly IInvoiceService _invoiceService;
    private readonly IPaymentService _paymentService;
    private readonly IOrderTableService _orderTableService;
    private readonly IOrderTaxService _orderTaxService;
    private readonly IOrderItemService _orderItemService;
    private readonly ITransactionRepository _transaction;
    private readonly IOrderStatusService _orderStatusService;

    public AppMenuService(IGenericRepository<Item> itemRepository, IGenericRepository<OrderTableMapping> orderTableRepository, ICategoryService categoryService, IOrderService orderService, IGenericRepository<Taxis> taxRepository, IGenericRepository<PaymentMethod> paymentMethodRepository, IUserService userService, IGenericRepository<Order> orderRepository, IGenericRepository<WaitingToken> waitingTokenRepository, IGenericRepository<OrderStatus> orderStatusRepository, IInvoiceService invoiceService, IPaymentService paymentService, IOrderTableService orderTableService, IOrderItemService orderItemService, IOrderTaxService orderTaxService, ITransactionRepository transaction, IOrderStatusService orderStatusService)
    {
        _itemRepository = itemRepository;
        _orderTableRepository = orderTableRepository;
        _categoryService = categoryService;
        _orderService = orderService;
        _taxRepository = taxRepository;
        _paymentMethodRepository = paymentMethodRepository;
        _userService = userService;
        _orderRepository = orderRepository;
        _waitingTokenRepository = waitingTokenRepository;
        _orderStatusRepository = orderStatusRepository;
        _invoiceService = invoiceService;
        _paymentService = paymentService;
        _orderTableService = orderTableService;
        _orderItemService = orderItemService;
        _orderTaxService = orderTaxService;
        _transaction = transaction;
        _orderStatusService = orderStatusService;

    }

    #region Get

    public async Task<AppMenuViewModel> Get(long customerId)
    {
        AppMenuViewModel appMenu = new()
        {
            Categories = await _categoryService.Get(),
            CustomerId = customerId,
            Taxes = _taxRepository.GetAll().ToList(),
            PaymentMethods = _paymentMethodRepository.GetAll().ToList(),
        };

        appMenu.Order.CustomerId = customerId;

        if (customerId == 0)
        {
            return appMenu;
        }
        else
        {
            IEnumerable<OrderTableMapping>? mapping = await _orderTableRepository.GetByCondition(
                predicate: otm => !otm.IsDeleted,
                includes: new List<Expression<Func<OrderTableMapping, object>>>
                {
                    otm => otm.Order
                },
                thenIncludes: new List<Func<IQueryable<OrderTableMapping>, IQueryable<OrderTableMapping>>>
                {
                    q => q.Include(otm => otm.Table)
                        .ThenInclude(t => t.Section).Where(otm => otm.CustomerId == customerId)
                }
            );

            appMenu.SectionName = mapping.Select(m => m.Table.Section.Name).FirstOrDefault()!;
            appMenu.Tables = mapping.Select(m => m.Table.Name).ToList();

            long? orderId = mapping.Select(m => m.OrderId).FirstOrDefault();

            if (orderId == null)
            {
                return appMenu;
            }

            appMenu.Order = await _orderService.Get((long)orderId);
            return appMenu;
        }
    }

    public async Task<List<ItemInfoViewModel>> List(long categoryId, string search)
    {
        IEnumerable<Item>? list = await _itemRepository.GetByCondition(
            predicate: i => !i.IsDeleted &&
                        (categoryId == 0
                        || categoryId == -1 && i.IsFavourite
                        || i.CategoryId == categoryId) &&
                        (string.IsNullOrEmpty(search) ||
                        i.Name.ToLower().Contains(search.ToLower())),
            orderBy: q => q.OrderBy(i => i.Id),
            includes: new List<Expression<Func<Item, object>>>
            {
                i => i.FoodType
            }
        );

        List<ItemInfoViewModel> items = list.Select(i => new ItemInfoViewModel
        {
            Id = i.Id,
            ImageUrl = i.ImageUrl,
            Name = i.Name,
            Type = i.FoodType.Name,
            Rate = i.Rate,
            IsFavourite = i.IsFavourite
        }).ToList();

        return items;
    }

    public async Task<ResponseViewModel> FavouriteItem(long itemId)
    {
        Item item = await _itemRepository.GetByIdAsync(itemId) ?? throw new NotFoundException(NotificationMessages.NotFound.Replace("{0}","Item"));

        item.IsFavourite = !item.IsFavourite;
        await _itemRepository.UpdateAsync(item);

        return new ResponseViewModel
        {
            Success = true,
            Message = NotificationMessages.Updated.Replace("{0}", "Item")
        };
    }

    #endregion Get

    #region  Save
    public async Task<ResponseViewModel> Save(OrderDetailViewModel orderVM)
    {
        try
        {
            await _transaction.BeginTransactionAsync();

            Order order = order = await _orderRepository.GetByIdAsync(orderVM.OrderId) ?? new Order();

            if (orderVM.OrderId == 0)
            {
                order.CustomerId = orderVM.CustomerId;
                order.StatusId = 1;
                order.CreatedBy = await _userService.LoggedInUser();
                order.Members = _waitingTokenRepository.GetByStringAsync(t => !t.IsDeleted && t.CustomerId == orderVM.CustomerId).Result!.Members;
            }

            //Create new Order if doesn't exist
            if (orderVM.OrderId == 0)
            {
                order.Id = await _orderRepository.AddAsyncReturnId(order);

                //Create Invoice and Update order in mapping
                await _invoiceService.Add(order.Id);
                await _orderTableService.Update(order.Id);
            }

            order.Instructions = orderVM.Comment;

            // Order Item
            await _orderItemService.Save(orderVM.ItemsList, order.Id);

            decimal subTotal = await _orderItemService.OrderItemTotal(order.Id);
            order.SubTotal = subTotal;
            await _orderRepository.UpdateAsync(order);


            //Tax on order item
            await _orderTaxService.Save(orderVM.Taxes, order.Id);
            decimal taxAmount = _orderTaxService.TotalTaxOnOrder(order.Id);

            order.FinalAmount = subTotal + taxAmount;
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
        ResponseViewModel response = new();

        // Order status - Completion
        Order? order = await _orderRepository.GetByIdAsync(orderId) ?? throw new NotFoundException(NotificationMessages.NotFound.Replace("{0}", "Order"));

        if (order.OrderItems.Any(oi => oi.ReadyQuantity != oi.Quantity))
        {
            response.Success = false;
            response.Message = NotificationMessages.CompleteOrderFailed;
            return response;
        }

        order.StatusId = await _orderStatusService.Get(SetOrderStatus.COMPLETED);
        await _orderRepository.UpdateAsync(order);

        // Delete Table assignment
        await _orderTableService.Delete(orderId);

        response.Success = true;
        return response;
    }

    public async Task<ResponseViewModel> CancelOrder(long orderId)
    {
        ResponseViewModel response = new();

        // Order status - Cancelled
        Order? order = await _orderRepository.GetByIdAsync(orderId) ?? throw new NotFoundException(NotificationMessages.NotFound.Replace("{0}", "Order"));

        if (order.OrderItems.Any(oi => oi.ReadyQuantity > 0))
        {
            response.Success = false;
            response.Message = NotificationMessages.CannotCancelOrder;
            return response;
        }

        order.StatusId = await _orderStatusService.Get(SetOrderStatus.CANCELLED);
        await _orderRepository.UpdateAsync(order);

        // Delete Table assignment
        await _orderTableService.Delete(orderId);
        response.Success = true;
        response.Message = NotificationMessages.Successfully.Replace("{0}", "Order Cancelled");
        return response;
    }

    #endregion Complete/Cancel
}

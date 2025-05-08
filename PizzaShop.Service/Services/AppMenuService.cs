
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Common;
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

    public AppMenuService(IGenericRepository<Item> itemRepository, IGenericRepository<OrderTableMapping> orderTableRepository, ICategoryService categoryService, IOrderService orderService, IGenericRepository<Taxis> taxRepository, IGenericRepository<PaymentMethod> paymentMethodRepository, IUserService userService, IGenericRepository<Order> orderRepository, IGenericRepository<WaitingToken> waitingTokenRepository, IGenericRepository<OrderStatus> orderStatusRepository, IInvoiceService invoiceService, IPaymentService paymentService, IOrderTableService orderTableService, IOrderItemService orderItemService)
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
    }

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
            else
            {
                appMenu.Order = await _orderService.Get((long)orderId);
                return appMenu;
            }
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
        Item? item = await _itemRepository.GetByIdAsync(itemId);

        ResponseViewModel response = new();

        if (item == null)
        {
            response.Success = false;
            response.Message = NotificationMessages.NotFound.Replace("{0}", "Item");
            return response;
        }

        item.IsFavourite = !item.IsFavourite;
        response.Success = await _itemRepository.UpdateAsync(item);
        response.Message = response.Success ? NotificationMessages.Updated.Replace("{0}", "Item") : NotificationMessages.UpdatedFailed.Replace("{0}", "Item");
        return response;
    }

    public async Task<ResponseViewModel> Save(OrderDetailViewModel orderVM)
    {
        try
        {
            Order? order = new();
            ResponseViewModel response = new();

            if (orderVM.OrderId == 0)
            {
                order.CustomerId = orderVM.CustomerId;
                order.StatusId = 1;
                order.CreatedBy = await _userService.LoggedInUser();
                order.Members = _waitingTokenRepository.GetByStringAsync(t => !t.IsDeleted && t.CustomerId == orderVM.CustomerId).Result!.Members;
            }
            else
            {
                order = await _orderRepository.GetByIdAsync(orderVM.OrderId);
                if (order == null)
                {
                    response.Success = false;
                    response.Message = NotificationMessages.NotFound.Replace("{0}", "Order");
                    return response;
                }
            }

            order.Instructions = orderVM.Comment;

            //Create new Order if doesn't exist
            if (orderVM.OrderId == 0)
            {
                orderVM.OrderId = await _orderRepository.AddAsyncReturnId(order);
                if (orderVM.OrderId < 1)
                {
                    response.Success = false;
                    return response;
                }

                //Create Invoice
                if (!await _invoiceService.Add(orderVM.OrderId))
                {
                    response.Success = false;
                    return response;
                }

                //Update order table mapping
                if (!await _orderTableService.Update(orderVM.OrderId))
                {
                    response.Success = false;
                    return response;
                }
            }
            else
            {
                response.Success = await _orderRepository.UpdateAsync(order);
                if (!response.Success)
                {
                    return response;
                }
            }

            // Order Item
            decimal subTotal = 0;
            response.Success = await _orderItemService.Save(orderVM.ItemsList, orderVM.OrderId);
            if (response.Success)
            {
                subTotal = _orderItemService.OrderItemTotal(orderVM.OrderId);
            }
            else
            {
                return response;
            }

            //Tax on order item
            decimal taxAmount = 0;
            response.Success = await _orderTaxService.Save(orderVM.Taxes, order.Id);
            if (response.Success)
            {
                taxAmount = _orderTaxService.TotalTaxOnOrder(orderVM.OrderId);
            }
            else
            {
                return response;
            }

            order.FinalAmount = subTotal + taxAmount;
            response.Success = await _orderRepository.UpdateAsync(order);
            if (!response.Success)
            {
                return response;
            }

            //Save Payment
            response.Success = await _paymentService.Save(orderVM.PaymentMethodId, orderVM.OrderId);
            if (!response.Success)
            {
                response.Success = false;
                return response;
            }

            response.Success = true;

            return response;
        }
        catch (Exception ex)
        {
            return new ResponseViewModel
            {
                Success = false,
                ExMessage = ex.Message,
            };
        }
    }

    public async Task<ResponseViewModel> CompleteOrder(long orderId)
    {
        ResponseViewModel response = new();

        // Order status - Completion
        Order? order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
        {
            response.Success = false;
            return response;
        }

        if (order.OrderItems.Any(oi => oi.ReadyQuantity != oi.Quantity))
        {
            response.Success = false;
            return response;
        }

        order.StatusId = _orderStatusRepository.GetByStringAsync(os => os.Name == "Completed").Result!.Id;
        response.Success = await _orderRepository.UpdateAsync(order);
        if (!response.Success)
        {
            return response;
        }

        // Delete Table assignment
        response.Success = await _orderTableService.Delete(orderId);
        return response;

    }


    public async Task<ResponseViewModel> CancelOrder(long orderId)
    {
        ResponseViewModel response = new();

        // Order status - Cancelled
        Order? order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
        {
            response.Success = false;
            return response;
        }

        if (order.OrderItems.Any(oi => oi.ReadyQuantity > 0))
        {
            response.Success = false;
            return response;
        }

        order.StatusId = _orderStatusRepository.GetByStringAsync(os => os.Name == "Cancelled").Result!.Id;
        response.Success = await _orderRepository.UpdateAsync(order);
        if (!response.Success)
        {
            return response;
        }

        // Delete Table assignment
        response.Success = await _orderTableService.Delete(orderId);
        return response;
    }
}

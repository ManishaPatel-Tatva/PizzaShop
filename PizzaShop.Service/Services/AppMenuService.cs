
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
    private readonly IItemService _itemService;
    private readonly IOrderService _orderService;
    private readonly IGenericRepository<Taxis> _taxRepository;
    private readonly IGenericRepository<PaymentMethod> _paymentMethodRepository;
    private readonly IUserService _userService;
    private readonly IGenericRepository<Order> _orderRepository;
    private readonly ICustomerService _customerService;
    private readonly IGenericRepository<OrderItem> _orderItemRepository;
    private readonly IGenericRepository<OrderItemsModifier> _orderItemsModifierRepository;
    private readonly IGenericRepository<OrderTaxMapping> _orderTaxRepository;
    private readonly IGenericRepository<WaitingToken> _waitingTokenRepository;
    private readonly IGenericRepository<Invoice> _invoiceRepository;
    private readonly IGenericRepository<Payment> _paymentRepository;

    public AppMenuService(IGenericRepository<Item> itemRepository, IGenericRepository<OrderTableMapping> orderTableRepository, ICategoryService categoryService, IItemService itemService, IOrderService orderService, IGenericRepository<Taxis> taxRepository, IGenericRepository<PaymentMethod> paymentMethodRepository, IUserService userService, IGenericRepository<Order> orderRepository, ICustomerService customerService, IGenericRepository<OrderItem> orderItemRepository, IGenericRepository<OrderItemsModifier> orderItemsModifierRepository, IGenericRepository<OrderTaxMapping> orderTaxRepository, IGenericRepository<WaitingToken> waitingTokenRepository, IGenericRepository<Invoice> invoiceRepository, IGenericRepository<Payment> paymentRepository)
    {
        _itemRepository = itemRepository;
        _orderTableRepository = orderTableRepository;
        _categoryService = categoryService;
        _itemService = itemService;
        _orderService = orderService;
        _taxRepository = taxRepository;
        _paymentMethodRepository = paymentMethodRepository;
        _userService = userService;
        _orderRepository = orderRepository;
        _customerService = customerService;
        _orderItemRepository = orderItemRepository;
        _orderItemsModifierRepository = orderItemsModifierRepository;
        _orderTaxRepository = orderTaxRepository;
        _waitingTokenRepository = waitingTokenRepository;
        _invoiceRepository = invoiceRepository;
        _paymentRepository = paymentRepository;
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

            decimal subTotal = 0;
            foreach (OrderItemViewModel? item in orderVM.ItemsList)
            {
                subTotal += item.Price * item.Quantity;
                foreach (ModifierViewModel? modifier in item.ModifiersList)
                {
                    subTotal += modifier.Rate * item.Quantity;
                }
            }

            order.SubTotal = subTotal;
            order.FinalAmount = subTotal;

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
                Invoice invoice = new(){
                    InvoiceNo = "#DOM" + DateTime.Today.Ticks,
                    OrderId = orderVM.OrderId,
                    CreatedBy = await _userService.LoggedInUser()
                };

                response.Success = await _invoiceRepository.AddAsync(invoice);
                if(!response.Success)
                {
                    return response;
                }

                //Create Payment
                Payment payment = new(){
                    OrderId = orderVM.OrderId,
                    PaymentMethodId = orderVM.PaymentMethodId
                };

                response.Success = await _paymentRepository.AddAsync(payment);
                if(!response.Success)
                {
                    return response;
                }

                //Update order table mapping
                var mappings = await _orderTableRepository.GetByCondition(ot => ot.CustomerId == orderVM.CustomerId && !ot.IsDeleted);
                foreach(var mapping in mappings)
                {
                    mapping.OrderId = orderVM.OrderId;
                    mapping.UpdatedBy = await _userService.LoggedInUser();
                    mapping.UpdatedAt = DateTime.Now;

                    response.Success = await _orderTableRepository.UpdateAsync(mapping);
                    if(!response.Success)
                    {
                        return response;
                    }
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

            List<long>? existingTaxes = _orderTaxRepository.GetByCondition(
                otm => otm.OrderId == orderVM.OrderId
            ).Result
            .Select(ot => ot.TaxId).ToList();

            List<long> removeTax = existingTaxes.Except(orderVM.TaxList.Select(t => t.TaxId)).ToList();

            foreach (long taxId in removeTax)
            {
                OrderTaxMapping? mapping = await _orderTaxRepository.GetByStringAsync(t => t.TaxId == taxId && t.OrderId == orderVM.OrderId);
                if (mapping == null)
                {
                    response.Success = false;
                    return response;
                }

                mapping.IsDeleted = true;
                mapping.UpdatedAt = DateTime.Now;
                mapping.UpdatedBy = await _userService.LoggedInUser();

                response.Success = await _orderTaxRepository.UpdateAsync(mapping);
                if (!response.Success)
                {
                    return response;
                }
            }

            decimal taxAmount = 0;
            foreach (long taxId in orderVM.Taxes)
            {
                Taxis? tax = await _taxRepository.GetByIdAsync(taxId);
                if (tax == null)
                {
                    response.Success = false;
                    return response;
                }

                OrderTaxMapping? taxMapping = await _orderTaxRepository.GetByStringAsync(ot => ot.TaxId == taxId && ot.OrderId == orderVM.OrderId);

                //If doesn't exist then create new tax
                taxMapping ??= new OrderTaxMapping
                {
                    OrderId = orderVM.OrderId,
                    TaxId = taxId,
                    CreatedBy = await _userService.LoggedInUser()
                };

                if (tax.IsEnabled)
                {
                    decimal taxOnOrder = (bool)tax.IsPercentage ? subTotal * tax.TaxValue / 100 : tax.TaxValue;
                    taxMapping.TaxValue = taxOnOrder;

                    response.Success = taxMapping.Id == 0 ? await _orderTaxRepository.AddAsync(taxMapping) : await _orderTaxRepository.UpdateAsync(taxMapping);
                    if (!response.Success)
                    {
                        response.Message = NotificationMessages.AddedFailed.Replace("{0}", "Tax");
                        return response;
                    }
                    taxAmount += taxOnOrder;
                }
            }

            order.FinalAmount = subTotal + taxAmount;
            response.Success = await _orderRepository.UpdateAsync(order);
            if (!response.Success)
            {
                return response;
            }

            List<long>? existingItems = _orderItemRepository.GetByCondition(
                predicate: oi => oi.OrderId == orderVM.OrderId && !oi.IsDeleted
            ).Result
            .Select(oi => oi.Id = oi.Id)
            .ToList();

            List<long>? removeItems = existingItems.Except(orderVM.ItemsList.Select(oi => oi.Id)).ToList();

            foreach (long orderItemId in removeItems)
            {
                OrderItem? removeItem = await _orderItemRepository.GetByIdAsync(orderItemId);
                removeItem.IsDeleted = true;
                removeItem.UpdatedBy = await _userService.LoggedInUser();
                removeItem.UpdatedAt = DateTime.Now;

                if (!await _orderItemRepository.UpdateAsync(removeItem))
                {
                    response.Success = false;
                    response.Message = NotificationMessages.AddedFailed.Replace("{0}", "Order Item");
                    return response;
                }
            }

            foreach (OrderItemViewModel? orderItem in orderVM.ItemsList)
            {
                OrderItem? existingItem = await _orderItemRepository.GetByIdAsync(orderItem.Id);
                if (existingItem == null)
                {
                    OrderItem newItem = new()
                    {
                        OrderId = orderVM.OrderId,
                        ItemId = orderItem.ItemId,
                        Quantity = orderItem.Quantity,
                        Price = orderItem.Price,
                        Instructions = orderItem.Instruction,
                        CreatedBy = await _userService.LoggedInUser(),
                        UpdatedAt = DateTime.Now,
                        UpdatedBy = await _userService.LoggedInUser()
                    };

                    newItem.Id = await _orderItemRepository.AddAsyncReturnId(newItem);

                    if (newItem.Id < 1)
                    {
                        response.Success = false;
                        return response;
                    }

                    foreach (ModifierViewModel? modifier in orderItem.ModifiersList)
                    {
                        if (!await SaveOrderItemModifier(modifier, newItem.Id))
                        {
                            response.Success = false;
                            response.Message = NotificationMessages.AddedFailed.Replace("{0}", "Order Item Modifier");
                        }
                    }

                    response.Success = true;
                }
                else
                {
                    existingItem.Quantity = orderItem.Quantity;
                    existingItem.Instructions = orderItem.Instruction;
                    existingItem.UpdatedAt = DateTime.Now;
                    existingItem.UpdatedBy = await _userService.LoggedInUser();

                    response.Success = await _orderItemRepository.UpdateAsync(existingItem);
                    return response;
                }
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

    // public async Task<bool> SaveOrderItem(OrderItemViewModel orderItemVM, long orderId)
    // {
    //     OrderItem? orderItem = new();
    //     if (orderItemVM.Id == 0)
    //     {
    //         orderItem.OrderId = orderId;
    //         orderItem.ItemId = orderItemVM.ItemId;
    //         orderItem.CreatedBy = await _userService.LoggedInUser();
    //     }
    //     else
    //     {
    //         orderItem = await _orderItemRepository.GetByIdAsync(orderItemVM.Id);
    //         if (orderItem == null)
    //         {
    //             return false;
    //         }
    //     }

    //     orderItem.Quantity = orderItemVM.Quantity;
    //     orderItem.Instructions = orderItemVM.Instruction;
    //     orderItem.Price = orderItemVM.Price;
    //     orderItem.UpdatedAt = DateTime.Now;
    //     orderItem.UpdatedBy = await _userService.LoggedInUser();

    //     if (orderItemVM.Id == 0)
    //     {
    //         orderItemVM.Id = await _orderItemRepository.AddAsyncReturnId(orderItem);
    //         if (orderItemVM.Id < 1)
    //         {
    //             return false;
    //         }

    //         foreach (var modifier in orderItemVM.ModifiersList)
    //         {
    //             if (!await SaveOrderItemModifier(modifier, orderItemVM.Id))
    //             {
    //                 return false;
    //             }
    //         }
    //     }
    //     else
    //     {
    //         return await _orderItemRepository.UpdateAsync(orderItem);
    //     }

    //     return false;
    // }

    public async Task<bool> SaveOrderItemModifier(ModifierViewModel modifier, long orderItemId)
    {
        OrderItemsModifier oim = new()
        {
            OrderItemId = orderItemId,
            ModifierId = modifier.Id,
            Price = modifier.Rate,
            CreatedBy = await _userService.LoggedInUser(),
            UpdatedAt = DateTime.Now,
            UpdatedBy = await _userService.LoggedInUser()
        };

        return await _orderItemsModifierRepository.AddAsync(oim);
    }

    public async Task<bool> SaveOrderItem(List<OrderItemViewModel> ItemsList, long orderId)
    {
        long createrId = await _userService.LoggedInUser();

        List<long> existingItem = _orderItemRepository.GetByCondition(
            oi => oi.OrderId == orderId && !oi.IsDeleted
        ).Result
        .Select(oi => oi.ItemId)
        .ToList();

        List<long> removeOrderItem = existingItem.Except(existingItem).ToList();

        foreach (var itemId in removeOrderItem)
        {
            OrderItem? orderItem = await _orderItemRepository.GetByStringAsync(oi => oi.OrderId == orderId && oi.ItemId == itemId && !oi.IsDeleted);

            if (orderItem == null)
            {
                return false;
            }

            orderItem.IsDeleted = true;
            orderItem.UpdatedBy = await _userService.LoggedInUser();
            orderItem.UpdatedAt = DateTime.Now;

            bool success = await _orderItemRepository.UpdateAsync(orderItem);
            if (!success)
            {
                return false;
            }

        }

        foreach (var item in ItemsList)
        {
            OrderItem orderItem = new()
            {
                ItemId = item.ItemId,
                Quantity = item.Quantity,
                Price = item.Price,
                CreatedBy = createrId,
                UpdatedBy = createrId
            };

            long orderItemId = await _orderItemRepository.AddAsyncReturnId(orderItem);
            if (orderItemId < 1)
            {
                return false;
            }

            foreach (var modifier in item.ModifiersList)
            {
                OrderItemsModifier oim = new()
                {
                    OrderItemId = orderItemId,
                    ModifierId = modifier.Id,
                    Quantity = 1,
                    Price = modifier.Rate,
                    CreatedBy = createrId,
                    UpdatedBy = createrId,
                    UpdatedAt = DateTime.Now,
                };

                if (await _orderItemsModifierRepository.AddAsync(oim))
                {
                    return false;
                }
            }
        }

        return true;
    }

}

using PizzaShop.Entity.Models;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Common;
using PizzaShop.Service.Exceptions;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Services;

public class OrderTaxService : IOrderTaxService
{
    private readonly IGenericRepository<Taxis> _taxRepository;
    private readonly IGenericRepository<Order> _orderRepository;
    private readonly IGenericRepository<OrderTaxMapping> _orderTaxRepository;
    private readonly IUserService _userService;

    public OrderTaxService(IGenericRepository<Taxis> taxRepository, IGenericRepository<Order> orderRepository, IGenericRepository<OrderTaxMapping> orderTaxRepository, IUserService userService)
    {
        _taxRepository = taxRepository;
        _orderRepository = orderRepository;
        _orderTaxRepository = orderTaxRepository;
        _userService = userService;

    }

    #region Save
    public async Task Save(long taxId, long orderId)
    {
        Taxis tax = await _taxRepository.GetByIdAsync(taxId)
                    ?? throw new NotFoundException(NotificationMessages.NotFound.Replace("{0}", "Item"));

        OrderTaxMapping? taxMapping = await _orderTaxRepository.GetByStringAsync(
                                    ot => ot.TaxId == taxId 
                                    && ot.OrderId == orderId 
                                    && !ot.IsDeleted)
                                    ?? new OrderTaxMapping
                                    {
                                        OrderId = orderId,
                                        TaxId = taxId,
                                        CreatedBy = await _userService.LoggedInUser()
                                    };

        if (tax.IsEnabled)
        {
            Order order = await _orderRepository.GetByIdAsync(orderId)
                        ?? throw new NotFoundException(NotificationMessages.NotFound.Replace("{0}", "Order"));

            taxMapping.TaxValue = (bool)tax.IsPercentage! ? order.SubTotal * tax.TaxValue / 100 : tax.TaxValue;

            if(taxMapping.Id == 0)
            {
                await _orderTaxRepository.AddAsync(taxMapping);
            }
            else
            {
                await _orderTaxRepository.UpdateAsync(taxMapping);
            }
        }
    }

    public async Task Save(List<long> taxes, long orderId)
    {
        List<long>? existingTaxes = _orderTaxRepository.GetByCondition(
                otm => otm.OrderId == orderId && !otm.IsDeleted
            ).Result
            .Select(ot => ot.TaxId).ToList();

        List<long> removeTax = existingTaxes.Except(taxes).ToList();

        foreach (long taxId in removeTax)
        {
            await Delete(taxId, orderId);
        }

        foreach (long taxId in taxes)
        {
            await Save(taxId, orderId);
        }

    }
    #endregion Save

    #region Delete

    public async Task Delete(long taxId, long orderId)
    {
        OrderTaxMapping mapping = await _orderTaxRepository.GetByStringAsync(t => t.TaxId == taxId && t.OrderId == orderId)
                                ?? throw new NotFoundException(NotificationMessages.NotFound.Replace("{0}", "Item"));

        mapping.IsDeleted = true;
        mapping.UpdatedAt = DateTime.Now;
        mapping.UpdatedBy = await _userService.LoggedInUser();

        await _orderTaxRepository.UpdateAsync(mapping);
    }

    #endregion Delete

    #region Common
    public decimal TotalTaxOnOrder(long orderId)
    {
        return _orderTaxRepository.GetByCondition(ot => ot.OrderId == orderId && !ot.IsDeleted).Result!.Sum(ot => ot.TaxValue);
    }
    #endregion Common


}

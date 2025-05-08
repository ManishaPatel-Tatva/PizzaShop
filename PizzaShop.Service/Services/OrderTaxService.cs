using PizzaShop.Entity.Models;
using PizzaShop.Repository.Interfaces;
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


    public async Task<bool> Save(List<long> taxes, long orderId)
    {
        bool success = false;
        decimal subTotal = _orderRepository.GetByIdAsync(orderId).Result!.SubTotal;

        List<long>? existingTaxes = _orderTaxRepository.GetByCondition(
                otm => otm.OrderId == orderId && !otm.IsDeleted
            ).Result
            .Select(ot => ot.TaxId).ToList();

        List<long> removeTax = existingTaxes.Except(taxes).ToList();

        foreach (long taxId in removeTax)
        {
            OrderTaxMapping? mapping = await _orderTaxRepository.GetByStringAsync(t => t.TaxId == taxId && t.OrderId == orderId);
            if (mapping == null)
            {
                return false;
            }

            mapping.IsDeleted = true;
            mapping.UpdatedAt = DateTime.Now;
            mapping.UpdatedBy = await _userService.LoggedInUser();

            success = await _orderTaxRepository.UpdateAsync(mapping);
            if (!success)
            {
                return false;
            }
        }

        foreach (long taxId in taxes)
        {
            Taxis? tax = await _taxRepository.GetByIdAsync(taxId);
            if (tax == null)
            {
                return false;
            }

            OrderTaxMapping? taxMapping = await _orderTaxRepository.GetByStringAsync(ot => ot.TaxId == taxId && ot.OrderId == orderId && !ot.IsDeleted);

            //If doesn't exist then create new tax
            taxMapping ??= new OrderTaxMapping
            {
                OrderId = orderId,
                TaxId = taxId,
                CreatedBy = await _userService.LoggedInUser()
            };

            if (tax.IsEnabled)
            {
                taxMapping.TaxValue = (bool)tax.IsPercentage! ? subTotal * tax.TaxValue / 100 : tax.TaxValue;

                success = taxMapping.Id == 0 ? await _orderTaxRepository.AddAsync(taxMapping) : await _orderTaxRepository.UpdateAsync(taxMapping);
                if (!success)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public decimal TotalTaxOnOrder(long orderId)
    {
        return _orderTaxRepository.GetByCondition(ot => ot.OrderId == orderId && !ot.IsDeleted).Result!.Sum(ot => ot.TaxValue);
    }


}

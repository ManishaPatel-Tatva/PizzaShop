using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IGenericRepository<Invoice> _invoiceRepository;
    private readonly IUserService _userService;

    public InvoiceService(IGenericRepository<Invoice> invoiceRepository, IUserService userService)
    {
        _invoiceRepository = invoiceRepository;
        _userService = userService;
    }

    public async Task<bool> Add(long orderId)
    {
        Invoice invoice = new()
        {
            InvoiceNo = "#DOM" + DateTime.Today.Ticks,
            OrderId = orderId,
            CreatedBy = await _userService.LoggedInUser()
        };

        return await _invoiceRepository.AddAsync(invoice);
    }

}

using PizzaShop.Entity.Models;
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

    public async Task Add(long orderId)
    {
        Invoice invoice = new()
        {
            InvoiceNo = "#DOM" + DateTime.Today.Ticks,
            OrderId = orderId,
            CreatedBy = await _userService.LoggedInUser()
        };

        await _invoiceRepository.AddAsync(invoice);
    }

}

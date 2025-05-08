namespace PizzaShop.Service.Interfaces;

public interface IInvoiceService
{
    Task<bool> Add(long orderId);
}

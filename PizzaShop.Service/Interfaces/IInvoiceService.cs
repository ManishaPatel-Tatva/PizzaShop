namespace PizzaShop.Service.Interfaces;

public interface IInvoiceService
{
    Task Add(long orderId);
}

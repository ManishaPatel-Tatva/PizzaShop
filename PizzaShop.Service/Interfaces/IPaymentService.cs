namespace PizzaShop.Service.Interfaces;

public interface IPaymentService
{
    Task<bool> Save(long PaymentMethodId, long orderId);
}

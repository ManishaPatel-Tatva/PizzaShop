namespace PizzaShop.Service.Interfaces;

public interface IPaymentService
{
    Task Save(long PaymentMethodId, long orderId);
}

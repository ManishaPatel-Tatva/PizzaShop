using PizzaShop.Entity.Models;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Services;

public class PaymentService : IPaymentService
{
    private readonly IGenericRepository<Payment> _paymentRepository;

    public PaymentService(IGenericRepository<Payment> paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task Save(long paymentMethodId, long orderId)
    {
        Payment payment = await _paymentRepository.GetByStringAsync(p => p.OrderId == orderId)
                        ?? new Payment
                        {
                            OrderId = orderId
                        };

        payment.PaymentMethodId = paymentMethodId;

        if (payment.Id == 0)
        {
            await _paymentRepository.AddAsync(payment);
        }
        else
        {
            await _paymentRepository.UpdateAsync(payment);
        }

    }

}

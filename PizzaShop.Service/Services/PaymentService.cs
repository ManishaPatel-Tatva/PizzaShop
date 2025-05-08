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

    public async Task<bool> Save(long paymentMethodId, long orderId)
    {
        Payment? payment = await _paymentRepository.GetByStringAsync(p => p.OrderId == orderId);

        payment ??= new Payment{
            OrderId = orderId
        };

        payment.PaymentMethodId = paymentMethodId;

        bool success = payment.Id == 0 ? await _paymentRepository.AddAsync(payment) : await _paymentRepository.UpdateAsync(payment);
        
        return success;
    }

}

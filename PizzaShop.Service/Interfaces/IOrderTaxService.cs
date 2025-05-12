namespace PizzaShop.Service.Interfaces;

public interface IOrderTaxService
{
    Task Save(long taxId, long orderId);
    Task Save(List<long> taxes, long orderId);
    Task Delete(long taxId, long orderId);
    decimal TotalTaxOnOrder(long orderId);
}

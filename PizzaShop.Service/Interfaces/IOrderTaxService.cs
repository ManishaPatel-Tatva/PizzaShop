namespace PizzaShop.Service.Interfaces;

public interface IOrderTaxService
{
    Task<bool> Save(List<long> taxes, long orderId);
    decimal TotalTaxOnOrder(long orderId);
}

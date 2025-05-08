namespace PizzaShop.Service.Interfaces;

public interface IOrderTableService
{
    Task<bool> Update(long orderId);
    Task<bool> Delete(long orderId);
}

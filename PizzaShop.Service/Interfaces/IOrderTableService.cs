namespace PizzaShop.Service.Interfaces;

public interface IOrderTableService
{
    Task Update(long orderId);
    Task Delete(long orderId);
}

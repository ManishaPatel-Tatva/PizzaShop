namespace PizzaShop.Service.Interfaces;

public interface IOrderStatusService
{
    Task<long> Get(string status);
}

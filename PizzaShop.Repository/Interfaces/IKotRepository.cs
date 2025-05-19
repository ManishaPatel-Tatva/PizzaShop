namespace PizzaShop.Repository.Interfaces;

public interface IKotRepository
{
    Task Update(bool isReady, int quantity, long id);
}

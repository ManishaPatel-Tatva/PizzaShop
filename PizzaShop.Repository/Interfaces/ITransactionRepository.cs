namespace PizzaShop.Repository.Interfaces;

public interface ITransactionRepository
{
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}

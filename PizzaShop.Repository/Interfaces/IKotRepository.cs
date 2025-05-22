using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Repository.Interfaces;

public interface IKotRepository
{
    Task Update(bool isReady, int quantity, long id);
    Task<IEnumerable<KotDbViewModel>> Get(long category_id, bool isReady);
}

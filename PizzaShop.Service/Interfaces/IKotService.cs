using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IKotService
{
    Task<List<CategoryViewModel>> Get();
    Task<KotViewModel> Get(long categoryId, int pageSize, int pageNumber, bool isReady);
}

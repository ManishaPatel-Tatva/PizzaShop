using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IKotService
{
    Task<KotViewModel> Get(long categoryId, int pageSize, int pageNumber, bool isReady);
    Task<ResponseViewModel> Update(KotCardViewModel kot);
}

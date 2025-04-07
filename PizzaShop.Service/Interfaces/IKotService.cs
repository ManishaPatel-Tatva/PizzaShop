using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IKotService
{
    Task<List<CategoryViewModel>> Get();
}

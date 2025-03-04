using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface ICategoryItemService
{
    List<CategoryViewModel> GetCategory();
}

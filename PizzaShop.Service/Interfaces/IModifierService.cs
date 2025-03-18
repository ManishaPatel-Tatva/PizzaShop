using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IModifierService
{
    List<ModifierGroupViewModel> GetModifierGroups();
}

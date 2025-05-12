using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IDashboardService
{
    DashboardViewModel Get(FilterViewModel filter);
}

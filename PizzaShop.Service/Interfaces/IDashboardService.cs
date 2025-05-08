using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IDashboardService
{
    Task<DashboardViewModel> Get(FilterViewModel filter);
}

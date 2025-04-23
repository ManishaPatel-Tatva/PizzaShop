using PizzaShop.Entity.Models;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Services;

public class AppMenuService : IAppMenuService
{
    private readonly IGenericRepository<Item> _itemRepository;

    public AppMenuService(IGenericRepository<Item> itemRepository)
    {
        _itemRepository = itemRepository;
    }

    // public async Task<>

}

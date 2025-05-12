using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Services;

public class OrderItemModifierService : IOrderItemModifierService
{
    private readonly IGenericRepository<OrderItemsModifier> _oimRepository;
    private readonly IUserService _userService;

    public OrderItemModifierService(IGenericRepository<OrderItemsModifier> oimRepository, IUserService userService)
    {
        _oimRepository = oimRepository;
        _userService = userService;

    }

    public async Task<bool> Add(List<ModifierViewModel> modifiers, long orderItemId)
    {
        foreach (ModifierViewModel? modifier in modifiers)
        {
            OrderItemsModifier oim = new()
            {
                OrderItemId = orderItemId,
                ModifierId = modifier.Id,
                Price = modifier.Rate,
                CreatedBy = await _userService.LoggedInUser(),
                UpdatedAt = DateTime.Now,
                UpdatedBy = await _userService.LoggedInUser()
            };

            await _oimRepository.AddAsync(oim);
        }
        return true;
    }

}

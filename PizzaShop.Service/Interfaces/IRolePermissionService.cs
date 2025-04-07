using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IRolePermissionService
{
    IEnumerable<Role> Get();
    Task<RolePermissionViewModel> Get(long roleId);
    Task<bool> Update(long roleId, List<PermissionViewModel> permissions, string createrEmail);
}

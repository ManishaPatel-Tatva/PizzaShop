using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IRolePermissionService
{
    IEnumerable<Role> GetAllRoles();

    RolePermissionViewModel GetRolePermission(long roleId);
    Task<bool> UpdateRolePermission(long roleId, List<PermissionViewModel> model);
}

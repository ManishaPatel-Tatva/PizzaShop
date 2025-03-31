using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IRolePermissionService
{
    IEnumerable<Role> GetAllRoles();

    RolePermissionViewModel GetRolePermissions(long roleId);
    Task<bool> UpdateRolePermission(long roleId, List<PermissionViewModel> model);
}

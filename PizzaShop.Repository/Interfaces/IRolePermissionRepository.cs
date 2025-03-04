using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Repository.Interfaces;

public interface IRolePermissionRepository
{
    RolePermissionViewModel GetRolePermissions(long roleId);

    Task<bool> UpdateRolePermission(long roleId, List<PermissionViewModel> model);
}

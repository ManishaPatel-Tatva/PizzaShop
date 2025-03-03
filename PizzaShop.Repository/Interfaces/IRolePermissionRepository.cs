using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Repository.Interfaces;

public interface IRolePermissionRepository
{
    RolePermissionViewModel GetRolePermissions(long roleId);
}

using Microsoft.EntityFrameworkCore;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;

namespace PizzaShop.Repository.Repositories;

public class RolePermissionRepository : IRolePermissionRepository
{
    private readonly PizzaShopContext _context;

    public RolePermissionRepository(PizzaShopContext context)
    {
        _context = context;
    }

    public RolePermissionViewModel GetRolePermissions(long roleId)
    {
        var selectedRole = _context.Roles.SingleOrDefault(u => u.Id == roleId);
        
        var rolePermission = _context.RolePermissions.Where(u => u.RoleId == selectedRole.Id);

        var permissionsVM = rolePermission
        .Include(u => u.Permission)
        .Select(u => new PermissionViewModel
        {
            PermissionId = u.PermissionId,
            PermissionName = u.Permission.Name,
            CanView = u.View,
            CanEdit = u.AddOrEdit,
            CanDelete = u.Delete,
        }).ToList();

        return new RolePermissionViewModel{
            Permissions = permissionsVM,
            RoleId = roleId,
            RoleName = selectedRole.Name
        };
    }
}

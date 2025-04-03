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
        Role? selectedRole = _context.Roles.SingleOrDefault(u => u.Id == roleId);
        
        IQueryable<RolePermission>? rolePermission = _context.RolePermissions.Where(u => u.RoleId == selectedRole.Id);

        List<PermissionViewModel>? permissionsVM = rolePermission
        .Include(u => u.Permission)
        .OrderBy(u => u.Permission.Id)
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

    public async Task<bool> UpdateRolePermission(long roleId, List<PermissionViewModel> model) 
    {
        try
        {
            foreach(var permission in model)
            {
                RolePermission? specificPermission = await _context.RolePermissions.FirstOrDefaultAsync(p => p.PermissionId == permission.PermissionId && p.RoleId == roleId);
                if (specificPermission != null){
                    specificPermission.View = permission.CanView;
                    specificPermission.AddOrEdit = permission.CanEdit;
                    specificPermission.Delete = permission.CanDelete;
                    _context.RolePermissions.Update(specificPermission);
                    await _context.SaveChangesAsync();
                }
            }

            return true;
        }
        catch(Exception)
        {
            return false;
        }
    }
}

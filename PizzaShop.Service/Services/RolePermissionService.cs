using PizzaShop.Entity.Models;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Interfaces;
using PizzaShop.Entity.ViewModels;
using System.Threading.Tasks;
using System.Linq.Expressions;
using PizzaShop.Service.Exceptions;
using PizzaShop.Service.Common;

namespace PizzaShop.Service.Services;

public class RolePermissionService : IRolePermissionService
{
    private readonly IGenericRepository<Role> _roleRepository;
    private readonly IGenericRepository<RolePermission> _rolePermissionRepository;
    private readonly IUserService _userService;

    // private readonly IRolePermissionRepository _rolePermissionRepository;


    public RolePermissionService(IGenericRepository<Role> roleRepository, IGenericRepository<RolePermission> rolePermissionRepository, IUserService userService)
    {
        _roleRepository = roleRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _userService = userService;

    }

    /*------------------- Role ---------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------*/
    public IEnumerable<Role> Get()
    {
        return _roleRepository.GetAll();
    }

    /*--------------------Permission-----------------------------------------------------------------------------------------
    --------------------------------------------------------------------------------------------------*/
    public async Task<RolePermissionViewModel> Get(long roleId)
    {
        Role selectedRole = await _roleRepository.GetByIdAsync(roleId)
                            ?? throw new NotFoundException(NotificationMessages.NotFound.Replace("{0}", "Role"));

        IEnumerable<RolePermission>? permissions = await _rolePermissionRepository.GetByCondition(
            rp => rp.RoleId == selectedRole.Id,
            orderBy: q => q.OrderBy(rp => rp.Permission.Id),
            includes: new List<Expression<Func<RolePermission, object>>>
            {
                rp => rp.Permission
            }
        );

        RolePermissionViewModel rolePermissions = new()
        {
            Permissions = permissions.Select(rp => new PermissionViewModel
            {
                PermissionId = rp.PermissionId,
                PermissionName = rp.Permission.Name,
                CanView = rp.View,
                CanEdit = rp.AddOrEdit,
                CanDelete = rp.Delete,
            }).ToList(),
            RoleId = roleId,
            RoleName = selectedRole.Name
        };

        return rolePermissions;
    }

    public async Task Update(long roleId, List<PermissionViewModel> permissions)
    {
        foreach (PermissionViewModel permission in permissions)
        {
            RolePermission Permission = _rolePermissionRepository.GetByCondition(p => p.PermissionId == permission.PermissionId && p.RoleId == roleId).Result.FirstOrDefault()
            ?? throw new NotFoundException(NotificationMessages.NotFound.Replace("{0}", "Role Permission"));

            Permission.View = permission.CanView;
            Permission.AddOrEdit = permission.CanEdit;
            Permission.Delete = permission.CanDelete;
            Permission.UpdatedBy = await _userService.LoggedInUser();
            Permission.UpdatedAt = DateTime.Now;

            await _rolePermissionRepository.UpdateAsync(Permission);
        }
    }

}

using PizzaShop.Entity.Models;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Interfaces;
using PizzaShop.Entity.ViewModels;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace PizzaShop.Service.Services;

public class RolePermissionService : IRolePermissionService
{
    private readonly IGenericRepository<Role> _roleRepository;
    private readonly IGenericRepository<RolePermission> _rolePermissionRepository;
    private readonly IGenericRepository<User> _userRepository;

    // private readonly IRolePermissionRepository _rolePermissionRepository;


    public RolePermissionService(IGenericRepository<Role> roleRepository, IGenericRepository<RolePermission> rolePermissionRepository, IGenericRepository<User> userRepository)
    {
        _roleRepository = roleRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _userRepository = userRepository;
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
        Role? selectedRole = await _roleRepository.GetByIdAsync(roleId);
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

    public async Task<bool> Update(long roleId, List<PermissionViewModel> permissions, string createrEmail)
    {
        User user= await _userRepository.GetByStringAsync(u => u.Email == createrEmail);

        foreach (PermissionViewModel? permission in permissions)
        {
            RolePermission? Permission =  _rolePermissionRepository.GetByCondition(p => p.PermissionId == permission.PermissionId && p.RoleId == roleId).Result.SingleOrDefault();
            if (Permission != null)
            {
                Permission.View = permission.CanView;
                Permission.AddOrEdit = permission.CanEdit;
                Permission.Delete = permission.CanDelete;
                Permission.UpdatedBy = user.Id;
                Permission.UpdatedAt = DateTime.Now;

                if (!await _rolePermissionRepository.UpdateAsync(Permission))
                {
                    return false;
                }
            }
        }
        return true;
    }

}

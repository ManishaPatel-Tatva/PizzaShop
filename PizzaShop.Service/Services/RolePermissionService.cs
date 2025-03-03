using PizzaShop.Entity.Models;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Interfaces;
using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Services;

public class RolePermissionService : IRolePermissionService
{
    private readonly IGenericRepository<Role> _roleRepository;
    private readonly IGenericRepository<Permission> _permissionRepository;
    // private readonly IGenericRepository<RolePermission> _rolePermissionRepository;

    private readonly IRolePermissionRepository _rolePermissionRepository;

    public RolePermissionService(IGenericRepository<Role> roleRepository, IGenericRepository<Permission> permissionRepository, IRolePermissionRepository rolePermissionRepository)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _rolePermissionRepository = rolePermissionRepository;
    }

    /*------------------- Role ---------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------*/
    public IEnumerable<Role> GetAllRoles()
    {
        return _roleRepository.GetAll();  
    }

    /*--------------------Permission-----------------------------------------------------------------------------------------
    --------------------------------------------------------------------------------------------------*/
    public RolePermissionViewModel GetRolePermission(long roleId)
    {
        return _rolePermissionRepository.GetRolePermissions(roleId);
    }
    
}

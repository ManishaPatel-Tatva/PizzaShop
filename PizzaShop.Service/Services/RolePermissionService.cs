using PizzaShop.Entity.Models;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Interfaces;
using PizzaShop.Entity.ViewModels;
using System.Threading.Tasks;

namespace PizzaShop.Service.Services;

public class RolePermissionService : IRolePermissionService
{
    private readonly IGenericRepository<Role> _roleRepository;


    private readonly IRolePermissionRepository _rolePermissionRepository;

    public RolePermissionService(IGenericRepository<Role> roleRepository, IRolePermissionRepository rolePermissionRepository)
    {
        _roleRepository = roleRepository;
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

    public async Task<bool> UpdateRolePermission(long roleId, List<PermissionViewModel> model)
    {
        return await _rolePermissionRepository.UpdateRolePermission(roleId, model);
    }
    
}

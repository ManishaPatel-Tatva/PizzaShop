using PizzaShop.Entity.Models;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Interfaces;
using PizzaShop.Entity.ViewModels;
using System.Threading.Tasks;

namespace PizzaShop.Service.Services;

public class RolePermissionService : IRolePermissionService
{
    private readonly IGenericRepository<Role> _roleRepository;
    private readonly IGenericRepository<RolePermission> _rolePermissionRepository;


    // private readonly IRolePermissionRepository _rolePermissionRepository;

    public RolePermissionService(IGenericRepository<Role> roleRepository, IGenericRepository<RolePermission> rolePermissionRepository)
    {
        _roleRepository = roleRepository;
        _rolePermissionRepository = rolePermissionRepository;
    }

    /*------------------- Role ---------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------*/
    public IEnumerable<Role> Get()
    {
        return _roleRepository.GetAll();  
    }

    /*--------------------Permission-----------------------------------------------------------------------------------------
    --------------------------------------------------------------------------------------------------*/
    // public RolePermissionViewModel Get(long roleId)
    // {
    //     return _rolePermissionRepository.GetRolePermissions(roleId);
    // }

    // public async Task<bool> Update(long roleId, List<PermissionViewModel> model)
    // {
    //     return await _rolePermissionRepository.UpdateRolePermission(roleId, model);
    // }

    public async Task<RolePermissionViewModel> Get(long roleId)
    {
        Role? selectedRole = await _roleRepository.GetByIdAsync(roleId);
        var rolePermissions = _rolePermissionRepository.GetByCondition(rp => rp.RoleId == selectedRole.Id);
        List<PermissionViewModel>? permissionsVM = _rolePermissionRepository.GetByCondition()

        
        return _rolePermissionRepository.GetRolePermissions(roleId);
    }

    public async Task<bool> Update(long roleId, List<PermissionViewModel> model)
    {
        return await _rolePermissionRepository.UpdateRolePermission(roleId, model);
    }
    
}

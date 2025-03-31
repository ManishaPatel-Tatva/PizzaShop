using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Service.Interfaces;
using PizzaShop.Web.Filters;

namespace PizzaShop.Web.Controllers;

[Authorize]
public class RolePermissionController : Controller
{
    private readonly IRolePermissionService _rolePermissionService;

    public RolePermissionController(IRolePermissionService rolePermissionService)
    {
        _rolePermissionService = rolePermissionService;
    }

#region Roles
/*---------------------------------------------------------Roles-----------------------------------------------------------------
--------------------------------------------------------------------------------------------------------------------------------*/
    [CustomAuthorize("View_Role and Permission")]
    [HttpGet]
    public IActionResult Role()
    {
        var roles = _rolePermissionService.GetAllRoles();
        ViewData["sidebar-active"] = "RolePermission";
        return View(roles);
    }
#endregion Roles

#region Permissions
/*---------------------------------------------------------Permission-----------------------------------------------------------------
--------------------------------------------------------------------------------------------------------------------------------*/
    [CustomAuthorize("View_Role and Permission")]
    [HttpGet]
    public IActionResult Permission(long id)
    {
        var model = _rolePermissionService.GetRolePermissions(id);
        ViewData["sidebar-active"] = "RolePermission";
        return View(model);
    }

    [CustomAuthorize("Edit_Role and Permission")]
    [HttpPost]
    public async Task<IActionResult> UpdatePermission(long roleId, List<PermissionViewModel> model)
    {
        
        var isUpdated = await _rolePermissionService.UpdateRolePermission(roleId, model);

        if (!isUpdated)
             return Json(new {success = false, message="Permission Not updated"});

        return Json(new {success = true, message="Permission updated Successfully!"});
    }
#endregion Permissions

}

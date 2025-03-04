using Microsoft.AspNetCore.Mvc;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Web.Controllers;

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

    [HttpGet]
    public IActionResult Role()
    {
        var roles = _rolePermissionService.GetAllRoles();
        ViewData["sidebar-active"] = "RolePermission";
        return View(roles);
    }

#endregion

#region Roles
/*---------------------------------------------------------Permission-----------------------------------------------------------------
--------------------------------------------------------------------------------------------------------------------------------*/
    [HttpGet]
    public IActionResult Permission(long id)
    {
        var model = _rolePermissionService.GetRolePermission(id);
        ViewData["sidebar-active"] = "RolePermission";
        return View(model);
    }


    [HttpPost]
    public async Task<IActionResult> UpdatePermission(long roleId, List<PermissionViewModel> model)
    {
        
        var isUpdated = await _rolePermissionService.UpdateRolePermission(roleId, model);

        return Json("Successful");
    }

#endregion

}

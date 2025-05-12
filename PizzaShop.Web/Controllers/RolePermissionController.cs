using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Service.Common;
using PizzaShop.Service.Interfaces;
using PizzaShop.Web.Filters;

namespace PizzaShop.Web.Controllers;

[Authorize]
public class RolePermissionController : Controller
{
    private readonly IRolePermissionService _rolePermissionService;
    private readonly IJwtService _jwtService;

    public RolePermissionController(IRolePermissionService rolePermissionService, IJwtService jwtService)
    {
        _rolePermissionService = rolePermissionService;
        _jwtService = jwtService;
    }

    #region Roles
    /*---------------------------------------------------------Roles-----------------------------------------------------------------
    --------------------------------------------------------------------------------------------------------------------------------*/

    [HttpGet]
    [CustomAuthorize(nameof(PermissionType.View_Roles_and_Permissions))]
    public IActionResult Role()
    {
        var roles = _rolePermissionService.Get();
        ViewData["sidebar-active"] = "RolePermission";
        return View(roles);
    }
    #endregion Roles

    #region Permissions
    /*---------------------------------------------------------Permission-----------------------------------------------------------------
    --------------------------------------------------------------------------------------------------------------------------------*/
    [HttpGet]
    [CustomAuthorize(nameof(PermissionType.View_Roles_and_Permissions))]
    public async Task<IActionResult> Permission(long id)
    {
        RolePermissionViewModel? permissions = await _rolePermissionService.Get(id);
        ViewData["sidebar-active"] = "RolePermission";
        return View(permissions);
    }

    [HttpPost]
    [CustomAuthorize(nameof(PermissionType.Edit_Roles_and_Permissions))]
    public async Task<IActionResult> Update(long roleId, List<PermissionViewModel> permissions)
    {
        await _rolePermissionService.Update(roleId, permissions);

        return Json(new ResponseViewModel
        {
            Success = true,
            Message = NotificationMessages.Updated.Replace("{0}", "Permission")
        });
    }
    #endregion Permissions

}

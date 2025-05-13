using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Service.Common;
using PizzaShop.Service.Interfaces;
using PizzaShop.Web.Filters;

namespace PizzaShop.Web.Controllers;

[Authorize]
public class TaxesController : Controller
{
    private readonly ITaxesFeesService _taxService;
    private readonly IJwtService _jwtService;

    public TaxesController(ITaxesFeesService taxService, IJwtService jwtService)
    {
        _taxService = taxService;
        _jwtService = jwtService;
    }

    /*---------------------------Display Users---------------------------------------------
    ---------------------------------------------------------------------------------------*/
    [CustomAuthorize(nameof(PermissionType.View_Taxes_and_Fees))]
    public IActionResult Index()
    {
        ViewData["sidebar-active"] = "Taxes";
        return View();
    }

    [HttpPost]
    [CustomAuthorize(nameof(PermissionType.View_Taxes_and_Fees))]
    public async Task<IActionResult> Get(FilterViewModel filter)
    {
        TaxPaginationViewModel model = await _taxService.Get(filter);
        return PartialView("_ListPartialView", model);
    }

    /*-------------------------------------------------------- Get Tax ---------------------------------------------------------------------------------------------------
    ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    [HttpGet]
    [CustomAuthorize(nameof(PermissionType.Edit_Taxes_and_Fees))]
    public async Task<IActionResult> Get(long taxId)
    {
        TaxViewModel model = await _taxService.Get(taxId);
        return PartialView("_TaxPartialView", model);
    }

    [HttpPost]
    [CustomAuthorize(nameof(PermissionType.Edit_Taxes_and_Fees))]
    public async Task<IActionResult> Save(TaxViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TaxViewModel updatedModel = await _taxService.Get(model.TaxId);
            return PartialView("_TaxPartialView", updatedModel);
        }

        ResponseViewModel response = await _taxService.Save(model);
        if (!response.Success)
        {
            TempData["NotificationMessage"] = response.Message;
            TempData["NotificationType"] = NotificationType.Error.ToString();
            TaxViewModel updatedModel = await _taxService.Get(model.TaxId);
            return PartialView("_TaxGroupPartialView", updatedModel);
        }
        return Json(response);
    }
   
    /*--------------------------------------------------------Delete One Tax--------------------------------------------------------------------------------------------------------
    ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    [HttpPost]
    [CustomAuthorize(nameof(PermissionType.Delete_Taxes_and_Fees))]
    public async Task<IActionResult> Delete(long taxId)
    {
        await _taxService.Delete(taxId);
        return Json(new ResponseViewModel
        {
            Success = true,
            Message = NotificationMessages.Deleted.Replace("{0}","Tax")
        });
    }

}

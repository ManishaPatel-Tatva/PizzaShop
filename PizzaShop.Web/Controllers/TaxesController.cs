using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Entity.ViewModels;
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
    [CustomAuthorize("View_Taxes and Fees")]
    public IActionResult Index()
    {
        ViewData["sidebar-active"] = "Taxes";
        return View();
    }

    [CustomAuthorize("View_Taxes and Fees")]
    [HttpPost]
    public async Task<IActionResult> GetAllTaxes(int pageSize, int pageNumber = 1, string search="")
    {
        TaxPaginationViewModel model = await _taxService.GetPagedTaxes(pageSize, pageNumber, search);
        if (model == null)
        {
            return NotFound(); // This triggers AJAX error
        }
        return PartialView("_TaxListPartialView", model);
    }

    /*-------------------------------------------------------- Get Tax ---------------------------------------------------------------------------------------------------
    ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    [CustomAuthorize("Edit_Taxes and Fees")]
    [HttpGet]
    public async Task<IActionResult> GetTaxModal(long taxId)
    {
        TaxViewModel model = await _taxService.GetTax(taxId);
        return PartialView("_TaxPartialView", model);
    }

    [CustomAuthorize("Edit_Taxes and Fees")]
    [HttpPost]
    public async Task<IActionResult> SaveTax(TaxViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TaxViewModel updatedModel = await _taxService.GetTax(model.TaxId);
            return PartialView("_TaxPartialView", updatedModel);
        }

        string token = Request.Cookies["authToken"];
        string createrEmail = _jwtService.GetClaimValue(token, "email");

        bool success = await _taxService.SaveTax(model, createrEmail);
        if (!success)
        {
            TaxViewModel updatedModel = await _taxService.GetTax(model.TaxId);
            return PartialView("_TaxGroupPartialView", updatedModel);
        }
        return Json(new { success = true, message = "Tax Added Successful!" });
    }
   
    /*--------------------------------------------------------Delete One Tax--------------------------------------------------------------------------------------------------------
    ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    [CustomAuthorize("Delete_Taxes and Fees")]
    [HttpPost]
    public async Task<IActionResult> DeleteTax(long taxId)
    {
        string token = Request.Cookies["authToken"];
        string createrEmail = _jwtService.GetClaimValue(token, "email");

        bool success = await _taxService.DeleteTax(taxId, createrEmail);

        if (!success)
        {
            return Json(new { success = false, message = "Tax Not deleted!" });
        }
        return Json(new { success = true, message = "Tax deleted Successfully!" });
    }

}

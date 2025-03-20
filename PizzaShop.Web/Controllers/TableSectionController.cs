using Microsoft.AspNetCore.Mvc;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Web.Controllers;

public class TableSectionController : Controller
{
    private readonly IJwtService _jwtService;
    private readonly ITableSectionService _tableSectionService;

    public TableSectionController(ITableSectionService tableSectionService, IJwtService jwtService)
    {
        _tableSectionService = tableSectionService;
        _jwtService = jwtService;
    }

    /*--------------------------------------------------------Table and Section Index---------------------------------------------------------------------------------------------------
    ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    [HttpGet]
    public IActionResult Index()
    {
        List<SectionViewModel> sectionList = _tableSectionService.GetAllSections();

        ViewData["sidebar-active"] = "TableSection";
        return View(sectionList);
    }

    #region Section

    #region Display Section
    /*-------------------------------------------------------- Get Section---------------------------------------------------------------------------------------------------
   ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    [HttpGet]
    public async Task<IActionResult> GetSectionModal(long sectionId)
    {
        SectionViewModel model = await _tableSectionService.GetSection(sectionId);
        return PartialView("_SectionPartialView", model);
    }
    #endregion Display Section

    #region Add/Update Section

    [HttpPost]
    public async Task<IActionResult> SaveSection(SectionViewModel model)
    {
        if (!ModelState.IsValid)
        {
            SectionViewModel updatedModel = await _tableSectionService.GetSection(model.SectionId);
            return PartialView("_SectionPartialView", updatedModel);
        }

        string token = Request.Cookies["authToken"];
        string createrEmail = _jwtService.GetClaimValue(token, "email");

        bool success = await _tableSectionService.SaveSection(model, createrEmail);
        if (!success)
        {
            SectionViewModel updatedModel = await _tableSectionService.GetSection(model.SectionId);
            return PartialView("_SectionPartialView", updatedModel);
        }
        return Json(new { success = true, message = "Section Added Successful!" });

    }
    #endregion Add/Update Section

    #region Delete Section

    [HttpGet]
    public async Task<IActionResult> DeleteSection(long sectionId)
    {
        bool success = await _tableSectionService.DeleteSection(sectionId);
        if (!success)
        {
            return Json(new { success = false, message = "Table Not deleted!" });
        }
        return Json(new { success = true, message = "Table deleted Successfully!" });
    }

    #endregion Delete Section

    #endregion Section

    #region Table

    #region Display Tables
    /*-------------------------------------------------------- Get Section---------------------------------------------------------------------------------------------------
   ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    [HttpGet]
    public async Task<IActionResult> GetTableModal(long tableId)
    {
        TableViewModel model = await _tableSectionService.GetTable(tableId);
        return PartialView("_TablePartialView", model);
    }

    [HttpGet]
    public async Task<IActionResult> GetTablesList(long sectionId, int pageSize, int pageNumber = 1, string search = "")
    {
        TablesPaginationViewModel model = await _tableSectionService.GetPagedTables(sectionId, pageSize, pageNumber, search);

        if (model == null)
        {
            return NotFound(); // This triggers AJAX error
        }

        return PartialView("_TableListPartialView", model);
    }

    #endregion Display Tables

    #region Add/Update Table

    [HttpPost]
    public async Task<IActionResult> SaveTable(TableViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TableViewModel updatedModel = await _tableSectionService.GetTable(model.TableId);
            return Json(new { success = false, message = "Table Not Added !" });
        }

        string token = Request.Cookies["authToken"];
        string createrEmail = _jwtService.GetClaimValue(token, "email");

        bool success = await _tableSectionService.SaveTable(model, createrEmail);
        if (!success)
        {
            TableViewModel updatedModel = await _tableSectionService.GetTable(model.TableId);
            return PartialView("_TablePartialView", updatedModel);
        }
        return Json(new { success = true, message = "Table Added Successfully!" });

    }
    #endregion Add/Update Table

    #region Delete Table
    /*--------------------------------------------------------Delete One Table--------------------------------------------------------------------------------------------------------
    ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<IActionResult> DeleteTable(long tableId)
    {
        bool success = await _tableSectionService.DeleteTable(tableId);

        if (!success)
        {
            return Json(new { success = false, message = "Table Not deleted!" });
        }
        return Json(new { success = true, message = "Table deleted Successfully!" });
    }

    /*--------------------------------------------------------Delete Multiple Tables--------------------------------------------------------------------------------------------------------
    ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public async Task<IActionResult> MassDeleteTable(List<long> modifierIdList)
    {
        bool success = await _tableSectionService.MassDeleteTables(modifierIdList);

        if (!success)
        {
            return Json(new { success = false, message = "Items Not deleted" });
        }
        return Json(new { success = true, message = "All selected Items deleted Successfully!" });
    }

    #endregion Delete Table

    #endregion Table

}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Service.Common;
using PizzaShop.Service.Interfaces;
using PizzaShop.Web.Filters;

namespace PizzaShop.Web.Controllers;

[Authorize]
public class TableSectionController : Controller
{
    private readonly ISectionService _sectionService;
    private readonly ITableService _tableService;

    public TableSectionController(ISectionService sectionService, ITableService tableService)
    {
        _sectionService = sectionService;
        _tableService = tableService;
    }

    /*--------------------------------------------------------Table and Section Index---------------------------------------------------------------------------------------------------
    ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    [HttpGet]
    [CustomAuthorize(nameof(PermissionType.View_Tables_and_Sections))]
    public IActionResult Index()
    {
        ViewData["sidebar-active"] = "TableSection";
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetSectionTab()
    {
        List<SectionViewModel> sections = await _sectionService.Get();
        return PartialView("_SectionTabPartialView", sections);
    }

    #region Section
    /*-------------------------------------------------------- Get Section---------------------------------------------------------------------------------------------------
   ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    [HttpGet]
    [CustomAuthorize(nameof(PermissionType.Edit_Tables_and_Sections))]
    public async Task<IActionResult> GetSectionModal(long sectionId)
    {
        SectionViewModel model = await _sectionService.Get(sectionId);
        return PartialView("_SectionPartialView", model);
    }

    [HttpPost]
    [CustomAuthorize(nameof(PermissionType.Edit_Tables_and_Sections))]
    public async Task<IActionResult> SaveSection(SectionViewModel sectionVM)
    {
        if (!ModelState.IsValid)
        {
            SectionViewModel updatedModel = await _sectionService.Get(sectionVM.Id);
            return PartialView("_SectionPartialView", updatedModel);
        }

        ResponseViewModel response = await _sectionService.Save(sectionVM);
        return Json(response);
    }

    [HttpGet]
    [CustomAuthorize(nameof(PermissionType.Delete_Tables_and_Sections))]
    public async Task<IActionResult> DeleteSection(long sectionId)
    {
        await _sectionService.Delete(sectionId);
        return Json(new ResponseViewModel
        {
            Success = true,
            Message = NotificationMessages.Deleted.Replace("{0}","Section")
        });
    }

    #endregion Section

    #region Table
    /*-------------------------------------------------------- Get Table---------------------------------------------------------------------------------------------------
   ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    [HttpGet]
    [CustomAuthorize(nameof(PermissionType.Edit_Tables_and_Sections))]
    public async Task<IActionResult> GetTableModal(long tableId)
    {
        TableViewModel model = await _tableService.Get(tableId);
        return PartialView("_TablePartialView", model);
    }

    [HttpGet]
    [CustomAuthorize(nameof(PermissionType.View_Tables_and_Sections))]
    public async Task<IActionResult> GetTablesList(long sectionId, FilterViewModel filter)
    {
        TablesPaginationViewModel tables = await _tableService.Get(sectionId, filter);
        return PartialView("_TableListPartialView", tables);
    }

    /*-------------------------------------------------------- Save Table---------------------------------------------------------------------------------------------------
      ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    [HttpPost]
    [CustomAuthorize(nameof(PermissionType.Edit_Tables_and_Sections))]
    public async Task<IActionResult> SaveTable(TableViewModel tableVM)
    {
        if (!ModelState.IsValid)
        {
            TableViewModel updatedTable = await _tableService.Get(tableVM.Id);
            return PartialView("_TablePartialView", updatedTable);
        }

        ResponseViewModel response = await _tableService.Save(tableVM);
        return Json(response);
    }

    /*--------------------------------------------------------Delete One Table--------------------------------------------------------------------------------------------------------
    ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    [CustomAuthorize(nameof(PermissionType.Delete_Tables_and_Sections))]
    public async Task<IActionResult> DeleteTable(long tableId)
    {
        await _tableService.Delete(tableId);
        return Json(new ResponseViewModel
        {
            Success = true,
            Message = NotificationMessages.Deleted.Replace("{0}","Table")
        });
    }

    /*--------------------------------------------------------Delete Multiple Tables--------------------------------------------------------------------------------------------------------
    ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    [CustomAuthorize(nameof(PermissionType.Delete_Tables_and_Sections))]
    public async Task<IActionResult> MassDeleteTable(List<long> tableIdList)
    {
        await _tableService.Delete(tableIdList);
        return Json(new ResponseViewModel
        {
            Success = true,
            Message = NotificationMessages.Deleted.Replace("{0}","Tables")
        });
    }

    #endregion Table

}

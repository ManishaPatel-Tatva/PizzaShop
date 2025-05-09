using Microsoft.AspNetCore.Mvc;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Web.Controllers;

public class WaitingListController : Controller
{
    private readonly IWaitingListService _waitingListService;
    private readonly IAppTableService _appTableService;

    public WaitingListController(IWaitingListService waitingListService, IAppTableService appTableService = null)
    {
        _waitingListService = waitingListService;
        _appTableService = appTableService;
    }

    public IActionResult Index()
    {
        ViewData["app-active"] = "Waiting List";
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetSectionTab()
    {
        List<SectionViewModel> sections = await _waitingListService.Get();
        return PartialView("_SectionListPartialView", sections);
    }

    [HttpGet]
    public async Task<IActionResult> GetWaitingList(long sectionId)
    {
        List<WaitingTokenViewModel>? list = await _waitingListService.List(sectionId);
        return PartialView("_ListPartialView", list);
    }

    [HttpGet]
    public async Task<IActionResult> WaitingTokenModal(long tokenId)
    {
        WaitingTokenViewModel token = await _waitingListService.Get(tokenId);
        return PartialView("_WaitingTokenPartialView", token);
    }

    [HttpGet]
    public async Task<IActionResult> AssignTableModal(long tokenId)
    {
        AssignTableViewModel assignTableVM = await _appTableService.Get(tokenId);
        return PartialView("_AssignTablePartialView", assignTableVM);
    }

    [HttpPost]
    public async Task<IActionResult> AssignTable(AssignTableViewModel assignTableVM)
    {   
        assignTableVM.Tables = assignTableVM.Tables.Where(t => t.IsSelected).ToList();
        ResponseViewModel response = await _appTableService.AssignTable(assignTableVM);
        return Json(response);
    }

    [HttpPost]
    public async Task<IActionResult> SaveWaitingToken(WaitingTokenViewModel tokenVM)
    {
       ResponseViewModel response = await _waitingListService.Save(tokenVM);
        return Json(response);
    }

    [HttpGet]
    public async Task<IActionResult> DeleteWaitingToken(long tokenId)
    {
        ResponseViewModel response = await _waitingListService.Delete(tokenId);
        return Json(response);
    }

}

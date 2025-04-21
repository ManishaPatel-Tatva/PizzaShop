using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Web.Controllers;

public class WaitingListController : Controller
{
    private readonly IWaitingListService _waitingListService;

    public WaitingListController(IWaitingListService waitingListService)
    {
        _waitingListService = waitingListService;
    }

    public async Task<IActionResult> Index()
    {
        List<SectionViewModel> sections = await _waitingListService.Get();
        ViewData["app-active"] = "Waiting List";
        return View(sections);
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

    [HttpPost]
    public async Task<IActionResult> SaveWaitingToken(WaitingTokenViewModel tokenVM)
    {
       ResponseViewModel response = await _waitingListService.Save(tokenVM);
        return Json(response);
    }

    public async Task<IActionResult> DeleteWaitingToken(long tokenId)
    {
        ResponseViewModel response = await _waitingListService.Delete(tokenId);
        return Json(response);
    }

}

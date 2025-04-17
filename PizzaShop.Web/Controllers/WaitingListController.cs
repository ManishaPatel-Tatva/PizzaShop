using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Web.Controllers;

public class WaitingListController : Controller
{
    private readonly ISectionService _sectionService;
    private readonly IWaitingListService _waitingListService;

    public WaitingListController(ISectionService sectionService, IWaitingListService waitingListService)
    {
        _sectionService = sectionService;
        _waitingListService = waitingListService;
    }

    public async Task<IActionResult> Index()
    {
        List<SectionViewModel> sections = await _sectionService.Get();
        ViewData["app-active"] = "Waiting List";
        return View(sections);
    }

    [HttpGet]
    public async Task<IActionResult> WaitingTokenModal(long tokenId)
    {
        WaitingTokenViewModel model = new()
        {
            Sections = await _sectionService.Get()
        };
        
        
        return PartialView("_WaitingTokenPartialView", model);
    }

    [HttpPost]
    public async Task<IActionResult> SaveWaitingToken(WaitingTokenViewModel tokenVM)
    {
       ResponseViewModel response = await _waitingListService.Save(tokenVM);
        return Json(response);
    }


    public async Task<IActionResult> GetWaitingList(long sectionId)
    {
        List<WaitingTokenViewModel>? list = await _waitingListService.List(sectionId);
        return PartialView("_ListPartialView", list);
    }
}

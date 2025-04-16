using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Web.Controllers;

[Authorize]
public class AppTableController : Controller
{
    private readonly IAppTableService _appTableService;
    private readonly ISectionService _sectionService;

    public AppTableController(IAppTableService appTableService, ISectionService sectionService)
    {
        _appTableService = appTableService;
        _sectionService = sectionService;
    }

    public async Task<ActionResult> Index()
    {
        ViewData["app-active"] = "Tables";
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        List<SectionViewModel> sections = await _appTableService.Get();
        return PartialView("_SectionListPartialView", sections);
    }

    [HttpGet]
    public async Task<IActionResult> WaitingTokenModal()
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
       ResponseViewModel response = await _appTableService.Save(tokenVM);
        return Json(response);
    }




}

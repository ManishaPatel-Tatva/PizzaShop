using System.Text.Json;
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
    private readonly IWaitingListService _waitingListService;

    public AppTableController(IAppTableService appTableService, ISectionService sectionService, IWaitingListService waitingListService)
    {
        _appTableService = appTableService;
        _sectionService = sectionService;
        _waitingListService = waitingListService;
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
    public async Task<IActionResult> OffCanvas(long sectionId)
    {
        AssignTableViewModel waitingToken = new()
        {
            WaitingList = await _waitingListService.List(sectionId)
        };
        waitingToken.CustomerDetail.Sections = await _sectionService.Get();
        waitingToken.CustomerDetail.SectionId = sectionId;

        return PartialView("_OffCanvasPartialView", waitingToken);
    }

    [HttpPost]
    public async Task<IActionResult> AssignTable(AssignTableViewModel token, string tableList)
    {
        if(!string.IsNullOrEmpty(tableList))
        {
            token.Tables = JsonSerializer.Deserialize<List<TableViewModel>>(tableList);
        }

        ResponseViewModel response = await _appTableService.Add(token);
        return Json(response);
    }




}

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

    public AppTableController(IAppTableService appTableService)
    {
        _appTableService = appTableService;
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
}

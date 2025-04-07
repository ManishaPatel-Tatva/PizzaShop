using Microsoft.AspNetCore.Mvc;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Web.Controllers;

public class KotController : Controller
{
    private readonly IKotService _kotService;

    public KotController(IKotService kotService)
    {
        _kotService = kotService;
    }

    public async Task<IActionResult> Index()
    {
        List<CategoryViewModel> list = await _kotService.Get();
        return View(list);
    }
}

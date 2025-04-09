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

    public async Task<IActionResult> GetCards(long categoryId, bool isReady, int pageNumber = 1, int pageSize = 4)
    {
        KotViewModel kot = await _kotService.Get(categoryId, pageSize, pageNumber, isReady);
        return PartialView("_CardsPartialView", kot);
    }
    public async Task<IActionResult> GetOrderItems(long orderId)
    {
        return PartialView("_OrderItemPartialView");
    }

}

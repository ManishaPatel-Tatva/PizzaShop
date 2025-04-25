using Microsoft.AspNetCore.Mvc;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Web.Controllers;

public class AppMenuController : Controller
{
    private readonly IAppMenuService _appMenuService;
    private readonly IItemService _itemService;

    public AppMenuController(IAppMenuService appMenuService, IItemService itemService)
    {
        _appMenuService = appMenuService;
        _itemService = itemService;
    }

    public async Task<ActionResult> Index(long id)
    {
        AppMenuViewModel appMenu = await _appMenuService.Get(id);
        ViewData["app-active"] = "Menu";
        return View(appMenu);
    }

    public async Task<IActionResult> GetCards(long categoryId, string search="")
    {
        List<ItemInfoViewModel> items = await _appMenuService.List(categoryId, search);
        return PartialView("_CardsPartialView", items);
    }

    [HttpPost]
    public async Task<IActionResult> FavouriteItem(long itemId)
    {
        ResponseViewModel response = await _appMenuService.FavouriteItem(itemId);
        return Json(response);
    }

    [HttpGet]
    public async Task<IActionResult> ItemDetail(long itemId)
    {
        AddItemViewModel item = await _itemService.GetEditItem(itemId);
        return PartialView("_ItemModifierPartialView",item);
    }

}

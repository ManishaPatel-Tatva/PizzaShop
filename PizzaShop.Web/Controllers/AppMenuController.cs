using Microsoft.AspNetCore.Mvc;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Web.Controllers;

public class AppMenuController : Controller
{
    private readonly ICategoryService _categoryService;
    private readonly IAppMenuService _appMenuService;
    private readonly IItemService _itemService;

    public AppMenuController(ICategoryService categoryService, IAppMenuService appMenuService, IItemService itemService)
    {
        _categoryService = categoryService;
        _appMenuService = appMenuService;
        _itemService = itemService;
    }

    public async Task<ActionResult> Index()
    {
        AppMenuViewModel appMenu = new()
        {
            Categories = await _categoryService.Get()
        };
        ViewData["app-active"] = "Menu";
        return View(appMenu);
    }

    public async Task<IActionResult> GetCards(long categoryId, string search="")
    {
        List<ItemInfoViewModel> items = await _appMenuService.Get(categoryId, search);
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

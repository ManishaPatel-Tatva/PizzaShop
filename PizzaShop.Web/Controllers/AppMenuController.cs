using Microsoft.AspNetCore.Mvc;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Web.Controllers;

public class AppMenuController : Controller
{
    private readonly ICategoryService _categoryService;

    public AppMenuController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }


    public async Task<ActionResult> IndexAsync()
    {
        List<CategoryViewModel> list = await _categoryService.Get();
        ViewData["app-active"] = "Menu";
        return View(list);
    }
}

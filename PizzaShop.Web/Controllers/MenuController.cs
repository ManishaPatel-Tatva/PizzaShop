using Microsoft.AspNetCore.Mvc;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Web.Controllers;

public class MenuController : Controller
{
    private readonly ICategoryItemService _categoryItemService;

    public MenuController(ICategoryItemService categoryItemService)
    {
        _categoryItemService = categoryItemService;
    }

    #region Menu Index
    /*--------------------------------------------------------Dashboard---------------------------------------------------------------------------------------------------
    ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    [HttpGet]
    public IActionResult Index()
    {
        var categoriesList = _categoryItemService.GetCategory();
        ViewData["sidebar-active"] = "Menu";
        return View(categoriesList);
    }
 

#endregion 

}

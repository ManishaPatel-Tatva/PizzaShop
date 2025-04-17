using Microsoft.AspNetCore.Mvc;

namespace PizzaShop.Web.Controllers;

public class AppMenuController : Controller
{
    public ActionResult Index()
    {
        ViewData["app-active"] = "Menu";
        return View();
    }
}

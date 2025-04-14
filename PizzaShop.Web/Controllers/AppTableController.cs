using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PizzaShop.Web.Controllers;

[Authorize]
public class AppTableController : Controller
{
    public ActionResult Index()
    {
        ViewData["app-active"] = "Tables";
        return View();
    }
}

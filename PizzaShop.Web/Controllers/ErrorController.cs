using Microsoft.AspNetCore.Mvc;

namespace PizzaShop.Web.Controllers;

public class ErrorController : Controller
{
    public ActionResult Index()
    {
        return View();
    }
}

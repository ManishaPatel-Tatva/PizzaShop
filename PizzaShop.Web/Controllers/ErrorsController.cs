using Microsoft.AspNetCore.Mvc;

namespace PizzaShop.Web.Controllers;

public class ErrorsController : Controller
{
    public ActionResult Index()
    {
        return View();
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Web.Controllers;

[Authorize(Roles ="Account Manager")]
public class EventsController : Controller
{
    
    // [HttpGet]
    // public IActionResult Index()
    // {
    //     OrderIndexViewModel model = _orderService.Get();
    //     ViewData["sidebar-active"] = "Orders";
    //     return View(model);
    // }
}

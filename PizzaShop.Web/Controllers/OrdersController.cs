using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Web.Controllers;

public class OrdersController : Controller
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<IActionResult> Index()
    {
        OrderIndexViewModel model = await _orderService.GetOrderIndex();
        ViewData["sidebar-active"] = "Orders";
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> GetOrdersList(int pageSize, int pageNumber = 1, string search="")
    {
        OrderPaginationViewModel model = await _orderService.GetPagedOrder(pageSize, pageNumber, search);
        if (model == null)
        {
            return NotFound(); // This triggers AJAX error
        }
        return PartialView("_OrdersListPartialView", model);
    }

}

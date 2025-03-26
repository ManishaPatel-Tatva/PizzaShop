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
    public async Task<IActionResult> GetOrdersList(string status, string dateRange, DateOnly? fromDate, DateOnly? toDate, string column="", string sort="", int pageSize=5, int pageNumber = 1, string search="")
    {
        OrderPaginationViewModel model = await _orderService.GetPagedOrder(status, dateRange, fromDate, toDate, column, sort, pageSize, pageNumber, search);
        if (model == null)
        {
            return NotFound(); // This triggers AJAX error
        }
        return PartialView("_OrdersListPartialView", model);
    }

    [HttpGet]
    public async Task<IActionResult> ExportOrderDetails(string status, string dateRange, DateOnly? fromDate, DateOnly? toDate, string column="", string sort="", string search="")
    {
        byte[] fileData = await _orderService.ExportOrderDetails(status, dateRange, fromDate, toDate, column, sort, search);
        return File(fileData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Orders.xlsx");
    }

    public IActionResult OrderDetails()
    {
        ViewData["sidebar-active"] = "Orders";
        return View();
    }



}

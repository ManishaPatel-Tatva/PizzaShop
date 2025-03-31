using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Service.Interfaces;
using PizzaShop.Web.Filters;

namespace PizzaShop.Web.Controllers;

[Authorize]
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

    [CustomAuthorize("View_Orders")]
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

    [CustomAuthorize("View_Orders")]
    [HttpGet]
    public async Task<IActionResult> ExportOrderDetails(string status, string dateRange, DateOnly? fromDate, DateOnly? toDate, string column="", string sort="", string search="")
    {
        byte[] fileData = await _orderService.ExportOrderDetails(status, dateRange, fromDate, toDate, column, sort, search);
        return File(fileData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Orders.xlsx");
    }

    [CustomAuthorize("View_Orders")]
    public async Task<IActionResult> OrderDetails(long orderId)
    {
        OrderDetailViewModel model = await _orderService.GetOrderDetail(orderId);
        ViewData["sidebar-active"] = "Orders";
        return View(model);
    }

    [CustomAuthorize("View_Orders")]
    public async Task<IActionResult> GenerateInvoice(long orderId)
    {
        byte[]? pdf = await _orderService.GenerateInvoice(orderId);
        return File(pdf, "application/pdf", "Order.pdf");;
    }



}

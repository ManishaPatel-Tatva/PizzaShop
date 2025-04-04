using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Service.Interfaces;
using PizzaShop.Web.Filters;
using Rotativa.AspNetCore;

namespace PizzaShop.Web.Controllers;

[Authorize]
public class OrdersController : Controller
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [CustomAuthorize("View_Orders")]
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        OrderIndexViewModel model = await _orderService.GetOrderIndex();
        ViewData["sidebar-active"] = "Orders";
        return View(model);
    }

    [CustomAuthorize("View_Orders")]
    [HttpPost]
    public async Task<IActionResult> GetOrdersList(FilterViewModel filter)
    {
        OrderPaginationViewModel model = await _orderService.GetPagedRecord(filter);
        if (model == null)
        {
            return NotFound(); // This triggers AJAX error
        }
        return PartialView("_OrdersListPartialView", model);
    }

    [CustomAuthorize("View_Orders")]
    [HttpGet]
    public async Task<IActionResult> ExportExcel(string status, string dateRange, DateOnly? fromDate, DateOnly? toDate, string column="", string sort="", string search="")
    {
        byte[] fileData = await _orderService.ExportExcel(status, dateRange, fromDate, toDate, column, sort, search);
        return File(fileData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Orders.xlsx");
    }

    [CustomAuthorize("View_Orders")]
    public async Task<IActionResult> OrderDetails(long orderId)
    {
        OrderDetailViewModel model = await _orderService.GetOrderDetail(orderId);
        ViewData["sidebar-active"] = "Orders";
        return View(model);
    }
    
    public async Task<IActionResult> Invoice(long orderId)
    {
        OrderDetailViewModel model = await _orderService.GetOrderDetail(orderId);
         ViewAsPdf? pdf = new("Invoice", model){
            FileName = "Invoice.pdf"
        };
        
        return pdf;

        return View(model);
    }

    public async Task<IActionResult> OrderDetailsPdf(long orderId)
    {
        OrderDetailViewModel model = await _orderService.GetOrderDetail(orderId);
         ViewAsPdf? pdf = new("OrderDetailsPdf", model){
            FileName = "Invoice.pdf"
        };
        
        return pdf;

        return View(model);
    }


}

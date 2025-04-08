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

    [HttpGet]
    [CustomAuthorize("View_Orders")]
    public async Task<IActionResult> Index()
    {
        OrderIndexViewModel model = await _orderService.Get();
        ViewData["sidebar-active"] = "Orders";
        return View(model);
    }

    [HttpPost]
    [CustomAuthorize("View_Orders")]
    public async Task<IActionResult> Get(FilterViewModel filter)
    {
        OrderPaginationViewModel model = await _orderService.Get(filter);
        return PartialView("_ListPartialView", model);
    }

    [HttpGet]
    [CustomAuthorize("View_Orders")]
    public async Task<IActionResult> ExportExcel(FilterViewModel filter)
    {
        byte[] fileData = await _orderService.ExportExcel(filter);
        return File(fileData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Orders.xlsx");
    }

    [CustomAuthorize("View_Orders")]
    public async Task<IActionResult> OrderDetails(long orderId)
    {
        OrderDetailViewModel model = await _orderService.Get(orderId);
        ViewData["sidebar-active"] = "Orders";
        return View(model);
    }
    
    public async Task<IActionResult> Invoice(long orderId)
    {
        OrderDetailViewModel model = await _orderService.Get(orderId);
        ViewAsPdf? pdf = new ( "Invoice" , model )
        {
            FileName = "Invoice.pdf"
        };    
        return pdf;
    }

    public async Task<IActionResult> ExportPdf(long orderId)
    {
        OrderDetailViewModel model = await _orderService.Get(orderId);
         ViewAsPdf? pdf = new("OrderDetailsPdf", model){
            FileName = "Invoice.pdf"
        };
        return pdf;
    }
}

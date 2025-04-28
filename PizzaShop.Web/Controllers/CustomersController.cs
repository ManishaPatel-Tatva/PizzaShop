using Microsoft.AspNetCore.Mvc;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Service.Interfaces;
using PizzaShop.Web.Filters;

namespace PizzaShop.Web.Controllers;

public class CustomersController : Controller
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    [CustomAuthorize("View_Customers")]
    public IActionResult Index()
    {
        ViewData["sidebar-active"] = "Customers";
        return View();
    }

    [HttpPost]
    [CustomAuthorize("View_Customers")]
    public async Task<IActionResult> Get(FilterViewModel filter)
    {
        CustomerPaginationViewModel customers = await _customerService.Get(filter);
        return PartialView("_ListPartialView", customers);
    }

    [HttpGet]
    [CustomAuthorize("View_Customers")]
    public async Task<IActionResult> GetCustomerHistory(long customerId)
    {
        CustomerHistoryViewModel customer = await _customerService.GetHistory(customerId);
        return PartialView("_CustomerHistoryPartialView", customer);
    }

    [HttpPost]
    [CustomAuthorize("View_Customers")]
    public async Task<IActionResult> ExportExcel(FilterViewModel filter)
    {
        byte[] fileData = await _customerService.ExportExcel(filter);
        return File(fileData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Customers.xlsx");
    }

    [HttpGet]
    public async Task<IActionResult> GetByEmail(string email)
    {
        CustomerViewModel customer = await _customerService.Get(email);
        return Json(customer);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(long id)
    {
        CustomerViewModel customer = await _customerService.Get(id);
        return PartialView("_CustomerDetailPartialView", customer);
    }

    [HttpPost]
    public async Task<IActionResult> SaveCustomer(CustomerViewModel customer)
    {
        ResponseViewModel response = await _customerService.Save(customer);
        return Json(response);
    }
}

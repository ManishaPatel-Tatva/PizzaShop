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

    [CustomAuthorize("View_Customers")]
    [HttpGet]
    public IActionResult Index()
    {
        ViewData["sidebar-active"] = "Customers";
        return View();
    }

    [CustomAuthorize("View_Customers")]
    [HttpGet]
    public async Task<IActionResult> GetCustomersList(string dateRange, DateOnly? fromDate, DateOnly? toDate, string column="", string sort="", int pageSize=5, int pageNumber = 1, string search="")
    {
        CustomerPaginationViewModel model = await _customerService.GetPagedCustomers(dateRange, fromDate, toDate, column, sort, pageSize, pageNumber, search);
        if (model == null)
        {
            return NotFound(); // This triggers AJAX error
        }
        return PartialView("_CustomersListPartialView", model);
    }

}

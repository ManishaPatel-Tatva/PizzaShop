using System.ComponentModel.DataAnnotations;

using PizzaShop.Models;
using PizzaShop.Services;
using PizzaShop.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace PizzaShop.Controllers;

public class DashboardController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly PizzashopContext _context;

    public DashboardController(ILogger<HomeController> logger, PizzashopContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult MyProfile()
    {

        
        return View();
    }

}
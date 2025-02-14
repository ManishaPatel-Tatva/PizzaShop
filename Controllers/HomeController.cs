using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaShop.Models;
using PizzaShop.ViewModel;

namespace PizzaShop.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly PizzashopContext _context;

    public HomeController(ILogger<HomeController> logger, PizzashopContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        

        if(ModelState.IsValid){
            var users = await _context.Users.Where(u => u.Email == model.Email).Select(x=> new{x.Email, x.Password}).FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);

            if(users != null && users.Password == model.Password){
                
                return RedirectToAction("Privacy");


            }
        
        }
        return View(model);

    }


        

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
        if(Request.Cookies["emailCookie"] != null)
        {
             return RedirectToAction("Privacy");
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        CookieOptions options = new CookieOptions();
        options.Expires = DateTime.Now.AddDays(15);

        if(ModelState.IsValid){
            var users = await _context.Users.Where(u => u.Email == model.Email).Select(x=> new{x.Email, x.Password}).FirstOrDefaultAsync();

            if(users != null && users.Password == model.Password){

                //For cookies               
                if(model.RememberMe)
                {
                    Response.Cookies.Append("emailCookie",model.Email,options);
                }

                return RedirectToAction("Privacy");
            }
        
        }
        return View(model);

    }

    public IActionResult ForgotPassword()
    {
        return View();
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

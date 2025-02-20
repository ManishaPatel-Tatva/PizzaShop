using System.ComponentModel.DataAnnotations;

using PizzaShop.Models;
using PizzaShop.Services;
using PizzaShop.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MailKit;
using Microsoft.EntityFrameworkCore;

namespace PizzaShop.Controllers;

public class DashboardController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly PizzashopContext _context;
    private readonly JwtService _jwtService;

    public DashboardController(ILogger<HomeController> logger, PizzashopContext context, JwtService jwtService)
    {
        _logger = logger;
        _context = context;
        _jwtService = jwtService;
    }

    public IActionResult MyProfile()
    {
        // email is fetched from the token
        var token = Request.Cookies["authToken"];
        var email = _jwtService.GetClaimValue(token,"email");

        // fetch user from database
        var user =  _context.Users.Where(u => u.Email == email).FirstOrDefault(); 

        //if user is null
        if (user == null)
        {
            return NotFound();
        }
        
        //
        MyProfileViewModel model = new MyProfileViewModel(); 
        model.FirstName = user.FirstName;
        model.LastName = user.LastName;
        model.UserName = user.Username;
        model.Email = user.Email;
        model.Phone = user.Phone;
        model.CountryID = user.CountryId;
        model.StateID = user.StateId;
        model.CityID = user.CityId;
        model.Address = user.Address;
        model.ZipCode = user.ZipCode;
        model.ProfileImageUrl = user.ProfileImg;

        model.Countries = _context.Countries.ToList();
        model.States = _context.States.Where(s => s.CountryId == user.CountryId).ToList();
        model.Cities = _context.Cities.Where(c => c.StateId == user.StateId).ToList();
         
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> MyProfile(MyProfileViewModel model)
    {
         if(!ModelState.IsValid){
            return View(model);
        }

        // email is fetched from the token
        var token = Request.Cookies["authToken"];
        var email = _jwtService.GetClaimValue(token,"email");

        var user = await _context.Users.FindAsync(model.Email);

        if (user == null)
        {
            return NotFound();
        }

        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.Username = model.UserName;
        user.Phone = model.Phone;
        // user.Country = model.Country;
        // user.State = model.State;
        // user.City = model.City;
        user.Address = model.Address;
        user.ZipCode = model.ZipCode;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Profile updated successfully!";
        return RedirectToAction("ChangePassword");
    }

    public IActionResult ChangePassword(){
        // email is fetched from the token
        var token = Request.Cookies["authToken"];
        var email = _jwtService.GetClaimValue(token,"email");
        ViewBag.email = email;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model){

        if(!ModelState.IsValid){
            return View(model);
        }

        // fetch user from database
        var user = await _context.Users.Where(u => u.Email == model.Email).FirstOrDefaultAsync();

        //if user is null
        if(user == null)
        {
            ModelState.AddModelError("","Email not found");
        }

        bool verified = BCrypt.Net.BCrypt.Verify(model.OldPassword, user.Password);  
        if(user != null && verified){
            if(model.NewPassword == model.NewPassword)
            {
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
                user.Password = passwordHash;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("MyProfile","Dashboard");
            }
            else{
                return View(model);
            }
        } 
        return View();
    }
}
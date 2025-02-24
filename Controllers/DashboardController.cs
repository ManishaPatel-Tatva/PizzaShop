using System.ComponentModel.DataAnnotations;

using PizzaShop.Models;
using PizzaShop.Services;
using PizzaShop.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MailKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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

    // Get Method for My Profile
    public IActionResult MyProfile()
    {
        // email is fetched from the token
        var token = Request.Cookies["authToken"];
        var email = _jwtService.GetClaimValue(token,"email");
        // var role = _jwtService.GetClaimValue(token,"role");

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
        model.CountryId = user.CountryId;
        model.StateId = user.StateId;
        model.CityId = user.CityId;
        model.Address = user.Address;
        model.ZipCode = user.ZipCode;
        model.ProfileImageUrl = user.ProfileImg;
        model.Role = _context.Roles.Where(q=> q.Id == user.RoleId).FirstOrDefault().Name;

        //Country, State and City
        ViewBag.countries = _context.Countries.ToList();
        ViewBag.states = _context.States.Where(s => s.CountryId == user.CountryId).ToList();
        ViewBag.cities = _context.Cities.Where(c => c.StateId == user.StateId).ToList();

         
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

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.Username = model.UserName;
        user.Phone = model.Phone;
        user.CountryId = model.CountryId;
        user.StateId = model.StateId;
        user.CityId = model.CityId;
        user.Address = model.Address;
        user.ZipCode = model.ZipCode;

        if (model.image != null)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

            //create folder if not exist
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string fileName = $"{Guid.NewGuid()}_{model.image.FileName}";
            string fileNameWithPath = Path.Combine(path, fileName);

            using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
            {
                model.image.CopyTo(stream);
            }
            user.ProfileImg = $"/uploads/{fileName}";
        }

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Profile updated successfully!";
        return RedirectToAction("ChangePassword", "Dashboard");
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

    public async Task<IActionResult> Logout()
    {
        
        // Delete the "Remember Me" cookie
        if (Request.Cookies["emailCookie"] != null)
        {
            Response.Cookies.Delete("emailCookie");
        }

        // await this._authService.LogoutAsync();
        return RedirectToAction("Login","Home");
    }


    [HttpGet]
    public IActionResult GetCountries()
    {
        var countries = _context.Countries.ToList();
        return Json(new SelectList(countries, "Id", "Name"));
    }

    [HttpGet]
    public IActionResult GetStates(int countryId)
    {
        var states = _context.States.Where(x => x.CountryId == countryId).ToList();
        return Json(new SelectList(states, "Id", "Name"));
    }
    [HttpGet]
    public IActionResult GetCities(int stateId)
    {
        var cities = _context.Cities.Where(x => x.StateId == stateId).ToList();
        return Json(new SelectList(cities, "Id", "Name"));
    }

    public IActionResult UsersList(){

        var allUsers = _context.Users.Include(x => x.Role).Select(u => new UserInfoViewModel{
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email,
            Phone = u.Phone,
            Role = u.Role.Name ,
            ProfileImageUrl = u.ProfileImg,
            Status =u.IsActive,
        }).ToList();

        UsersListViewModel model = new UsersListViewModel();
        model.User = allUsers;

        return View(model);
    }

    [HttpGet]
    public IActionResult AddUser(){
        ViewBag.roles = _context.Roles.ToList();
        ViewBag.countries = _context.Countries.ToList();
        ViewBag.states = _context.States.ToList();
        ViewBag.cities = _context.Cities.ToList();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddUser(MyProfileViewModel model)
    {

        if(!ModelState.IsValid){
            return View(model);
        }

        // email is fetched from the token
        var token = Request.Cookies["authToken"];
        var email = _jwtService.GetClaimValue(token,"email");

        var user = await _context.Users.FirstOrDefaultAsync();

        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.Username = model.UserName;
        user.Phone = model.Phone;
        user.CountryId = model.CountryId;
        user.StateId = model.StateId;
        user.CityId = model.CityId;
        user.Address = model.Address;
        user.ZipCode = model.ZipCode;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Profile updated successfully!";
        return RedirectToAction("ChangePassword", "Dashboard");
    }

}
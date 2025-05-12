using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Service.Common;
using PizzaShop.Service.Exceptions;
using PizzaShop.Service.Interfaces;
using PizzaShop.Web.Filters;

namespace PizzaShop.Web.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly IProfileService _profileService;
    private readonly IJwtService _jwtService;
    private readonly IAddressService _addressService;
    private readonly IDashboardService _dashboardService;

    public ProfileController(IProfileService profileService, IJwtService jwtService, IAddressService addressService, IDashboardService dashboardService)
    {
        _profileService = profileService;
        _jwtService = jwtService;
        _addressService = addressService;
        _dashboardService = dashboardService;
    }

    #region Dashboard
    /*--------------------------------------------------------Dashboard---------------------------------------------------------------------------------
    ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    [HttpGet]
    [CustomAuthorize("View_Orders")]
    public IActionResult Dashboard()
    {
        if (User.IsInRole("Chef"))
        {
            return RedirectToAction("Index", "Kot");
        }

        ViewData["sidebar-active"] = "Dashboard";
        return View();
    }

    // POST DashBoardPartial
    [HttpPost]
    public IActionResult DashBoardPartial(FilterViewModel filter)
    {
        if (filter.FromDate.HasValue && filter.ToDate.HasValue && filter.FromDate > filter.ToDate)
        {
            return Json(new { success = false, message = "FromDate must be less then ToDate" });
        }

        return PartialView("_DashboardPartialView", _dashboardService.Get(filter));
    }
    #endregion Dashboard


    #region My Profile
    /*------------------------------------------------------ View My Profile and Update Profile---------------------------------------------------------------------------------
    ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    [HttpGet]
    public async Task<IActionResult> MyProfile()
    {
        ProfileViewModel profile = await _profileService.Get();
        return View(profile);
    }

    [HttpPost]
    public async Task<IActionResult> MyProfile(ProfileViewModel model)
    {
        ProfileViewModel profileModel = await _profileService.Get();

        if (!ModelState.IsValid)
        {
            return View(profileModel);
        }

        await _profileService.Update(model);

        TempData["NotificationMessage"] = NotificationMessages.Updated.Replace("{0}", "Profile");
        TempData["NotificationType"] = NotificationType.Success.ToString();

        return RedirectToAction("Dashboard");
    }

    #endregion My Profile

    #region Address
    /*------------------------------------------------------ Country, state and City---------------------------------------------------------------------------------
    ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    [HttpGet]
    public IActionResult GetCountries()
    {
        List<Country>? countries = _addressService.GetCountries();
        return Json(new SelectList(countries, "Id", "Name"));
    }

    [HttpGet]
    public IActionResult GetStates(long countryId)
    {
        List<State>? states = _addressService.GetStates(countryId);
        return Json(new SelectList(states, "Id", "Name"));
    }

    [HttpGet]
    public IActionResult GetCities(long stateId)
    {
        List<City>? cities = _addressService.GetCities(stateId);
        return Json(new SelectList(cities, "Id", "Name"));
    }

    #endregion Address


    #region Change Password       
    /*---------------------------------------------------------------Change Password---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        ResponseViewModel response = await _profileService.ChangePassword(model);

        TempData["NotificationMessage"] = response.Message;
        if (response.Success)
        {
            TempData["NotificationType"] = NotificationType.Success.ToString();
            return RedirectToAction("Logout");
        }
        else
        {
            TempData["NotificationType"] = NotificationType.Error.ToString();
            return View(model);
        }
    }
    #endregion


    #region Logout
    /*---------------------------------------------------------------Logout---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public IActionResult Logout()
    {
        // Delete the "Remember Me" cookie
        if (Request.Cookies["emailCookie"] != null)
        {
            Response.Cookies.Delete("authToken");
            Response.Cookies.Delete("emailCookie");
            Response.Cookies.Delete("profileImg");
            Response.Cookies.Delete("userName");
        }
        return RedirectToAction("Login", "Auth");
    }

    #endregion

}



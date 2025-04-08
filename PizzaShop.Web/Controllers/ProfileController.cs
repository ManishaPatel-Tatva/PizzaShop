using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Service.Common;
using PizzaShop.Service.Interfaces;
using PizzaShop.Web.Filters;

namespace PizzaShop.Web.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly IProfileService _profileService;
    private readonly IJwtService _jwtService;
    private readonly IAddressService _addressService;

    public ProfileController(IProfileService profileService, IJwtService jwtService, IAddressService addressService)
    {
        _profileService = profileService;
        _jwtService = jwtService;
        _addressService = addressService;
    }

    #region Dashboard
    /*--------------------------------------------------------Dashboard---------------------------------------------------------------------------------
    ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    [CustomAuthorize("View_Orders")]
    [HttpGet]
    public IActionResult Dashboard()
    {
        if (User.IsInRole("Chef"))
        {
            return RedirectToAction("Index", "Kot");
        }

        ViewData["sidebar-active"] = "Dashboard";
        return View();
    }
    #endregion Dashboard


    #region My Profile
    /*------------------------------------------------------ View My Profile and Update Profile---------------------------------------------------------------------------------
    ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    [HttpGet]
    public async Task<IActionResult> MyProfile()
    {
        string token = Request.Cookies["authToken"];
        string email = _jwtService.GetClaimValue(token, "email");

        ProfileViewModel profile = await _profileService.Get(email);
        return View(profile);
    }

    [HttpPost]
    public async Task<IActionResult> MyProfile(ProfileViewModel model)
    {
        string profileToken = Request.Cookies["authToken"];
        string profileEmail = _jwtService.GetClaimValue(profileToken, "email");
        ProfileViewModel profileModel = await _profileService.Get(profileEmail);

        if (!ModelState.IsValid)
        {
            return View(profileModel);
        }

        if (await _profileService.Update(model))
        {
            TempData["NotificationMessage"] = NotificationMessages.Updated.Replace("{0}", "Profile");
            TempData["NotificationType"] = NotificationType.Success.ToString();
        }
        else
        {
            TempData["NotificationMessage"] = NotificationMessages.UpdatedFailed.Replace("{0}", "Profile");
            TempData["NotificationType"] = NotificationType.Error.ToString();
            return View(profileModel);
        }

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

        string token = Request.Cookies["authToken"];
        model.Email = _jwtService.GetClaimValue(token, "email");

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



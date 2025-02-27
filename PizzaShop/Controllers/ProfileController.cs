using DataAccessLayer.ViewModels;
using BusinessLogicLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BusinessLogicLayer.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PizzaShop.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IProfileService _profileService;
        private readonly JwtService _jwtService;
        private readonly ICountryService _countryService;

        public ProfileController(IProfileService profileService, JwtService jwtService,ICountryService countryService)
        {
            _profileService = profileService;
            _jwtService = jwtService;
            _countryService = countryService;
        }

/*------------------------------------------------------ View My Profile and Update Profile---------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
#region My Profile

        [HttpGet]
        public async Task<IActionResult> MyProfile()
        {
            var token = Request.Cookies["authToken"];
            var email = _jwtService.GetClaimValue(token, "email");

            var model = await _profileService.GetMyProfileAsync(email);
            if (model == null) 
                return NotFound();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> MyProfile(MyProfileViewModel model)
        {
            if (!ModelState.IsValid) 
                return View(model);

            var isUpdated = await _profileService.UpdateProfileAsync(model);

            if (!isUpdated) 
                return View(model);


            return RedirectToAction("UsersList","ManageUsers");
        }

#endregion

#region Country, state and City
[HttpGet]
    public IActionResult GetCountries()
    {
        var countries = _countryService.GetCountries();
        return Json(new SelectList(countries, "Id", "Name"));
    }

    [HttpGet]
    public IActionResult GetStates(long countryId)
    {
        var states = _countryService.GetStates(countryId);
        return Json(new SelectList(states, "Id", "Name"));
    }

    [HttpGet]
    public IActionResult GetCities(long stateId)
    {
        var cities = _countryService.GetCities(stateId);
        return Json(new SelectList(cities, "Id", "Name"));
    }

    // [HttpGet]
    // public IActionResult GetRoles(int roleID)
    // {
    //     var roles = _context.Roles.ToList();
    //     return Json(new SelectList(roles, "Id", "Name"));
    // }
#endregion

/*---------------------------------------------------------------Change Password---------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------------------*/
#region Change Password       

        [HttpGet]
        public IActionResult ChangePassword()
        {
            var token = Request.Cookies["authToken"];
            var email = _jwtService.GetClaimValue(token, "email");
            ViewBag.email = email;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var isPasswordChanged = await _profileService.ChangePasswordAsync(model);
            if (!isPasswordChanged) return View(model);

            TempData["SuccessMessage"] = "Password changed successfully!";
            return RedirectToAction("MyProfile");
        }

#endregion

/*---------------------------------------------------------------Logout---------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------------------*/
#region Logout

        public IActionResult Logout()
        {
            
            // Delete the "Remember Me" cookie
            if (Request.Cookies["emailCookie"] != null)
            {
                Response.Cookies.Delete("emailCookie");
            }

            // await this._authService.LogoutAsync();
            return RedirectToAction("Login","Auth");
        }

#endregion

    }
}

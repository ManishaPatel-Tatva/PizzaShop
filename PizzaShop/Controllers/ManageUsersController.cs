using BusinessLogicLayer.Helpers;
using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Services;
using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace PizzaShop.Controllers
{
    public class ManageUsersController : Controller
    {
        private readonly IUserService _userService;
         private readonly JwtService _jwtService;
        private readonly ICountryService _countryService;

        public ManageUsersController(IUserService userService,JwtService jwtService, ICountryService countryService)
        {
            _userService = userService;
            _jwtService = jwtService;
            _countryService = countryService;
        }


/*---------------------------Display Users---------------------------------------------
---------------------------------------------------------------------------------------*/
#region Display User
        public async Task<IActionResult> UsersList()
        {
            var model = await _userService.GetUsersListAsync();
            return View(model);
        }
#endregion

/*---------------------------Country State City Role Dropdown---------------------------------------------
---------------------------------------------------------------------------------------*/
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

        [HttpGet]
        public IActionResult GetRoles()
        {
            var roles = _userService.GetRoles();
            return Json(new SelectList(roles, "Id", "Name"));
        }
#endregion

/*---------------------------Add User---------------------------------------------
---------------------------------------------------------------------------------------*/
#region Add user
        [HttpGet]
        public async Task<IActionResult> AddUser()
        {
            var model = await _userService.GetAddUser();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(AddUserViewModel model)
        {
            if (!ModelState.IsValid) 
                return View(model);

            var token = Request.Cookies["authToken"];

            await _userService.AddUserAsync(model, token);
            bool success = true;
            if (success) TempData["SuccessMessage"] = "User added successfully!";

            return RedirectToAction("UsersList");
        }
#endregion

/*---------------------------Add User---------------------------------------------
---------------------------------------------------------------------------------------*/
#region Edit User

        [HttpGet]
        public IActionResult EditUser(long userId)
        {
            var model =  _userService.GetUserByIdAsync(userId);
            if (model == null) 
                return NotFound();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            if (!ModelState.IsValid) 
                return View(model);

            var isUpdated = await _userService.UpdateUser(model);

            if (!isUpdated) 
                return View(model);

            return RedirectToAction("UsersList","ManageUsers");
        }
#endregion


/*-------------------------------------Soft Delete User-------------------------------------------------------
-------------------------------------------------------------------------------------------------------*/
#region Soft Delete

        [HttpPost]
        public async Task<IActionResult> SoftDeleteUser(long id)
        {
            bool success = await _userService.SoftDeleteUser(id);

            // if (success) 
            //     TempData["SuccessMessage"] = "User deleted successfully!";

            return RedirectToAction("UsersList");
        }

#endregion 

    }
}

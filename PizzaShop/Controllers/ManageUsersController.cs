using BusinessLogicLayer.Helpers;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PizzaShop.Controllers
{
    public class ManageUsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly ICountryService _countryService; 
        private readonly JwtService _jwtService;

        public ManageUsersController(IUserService userService, JwtService jwtService, ICountryService countryService)
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
            ViewData["sidebar-active"] = "Users";
            return View();
        }

        [HttpGet]
        public IActionResult UsersListPage(int pageNumber, int pageSize, string search)
        {
            return Json(_userService.GetUsersListAsync(pageNumber, pageSize, search));
        }


#endregion

/*---------------------------Add User---------------------------------------------
---------------------------------------------------------------------------------------*/
#region Add user
        [HttpGet]
        public async Task<IActionResult> AddUser()
        {
            var model = await _userService.GetAddUser();
            ViewData["sidebar-active"] = "Users";
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(AddUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["sidebar-active"] = "Users";
                return View(model);
            }
                
            var token = Request.Cookies["authToken"];
            var createrEmail = _jwtService.GetClaimValue(token, "email");

            await _userService.AddUserAsync(model, createrEmail);
            bool success = true;
            if (success) 
                TempData["SuccessMessage"] = "User added successfully!";

            return RedirectToAction("UsersList");
        }
#endregion

/*---------------------------Edit User---------------------------------------------
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
#region Soft Delete User

        [HttpPost]
        public async Task<IActionResult> SoftDeleteUser(long id)
        {
            bool success = await _userService.SoftDeleteUser(id);
            return RedirectToAction("UsersList");
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

    }
}

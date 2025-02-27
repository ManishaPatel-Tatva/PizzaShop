using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Services;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace PizzaShop.Controllers
{
    public class ManageUsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly ICountryService _countryService;

        public ManageUsersController(IUserService userService, ICountryService countryService)
        {
            _userService = userService;
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

/*---------------------------Country State City---------------------------------------------
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

    // [HttpGet]
    // public IActionResult GetRoles(int roleID)
    // {
    //     var roles = _context.Roles.ToList();
    //     return Json(new SelectList(roles, "Id", "Name"));
    // }
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
        public async Task<IActionResult> AddUser(User user)
        {
            if (!ModelState.IsValid) return View(user);


            // bool success = await _userService.AddUserAsync(user, token);
            bool success = true;
            if (success) TempData["SuccessMessage"] = "User added successfully!";

            return RedirectToAction("UsersList");
        }
#endregion

#region Edit User

        // [HttpGet]
        // public IActionResult EditUser()
        // {
        //     var user =  _userService.GetUserByIdAsync(id);
        //     if (user == null) return NotFound();

        //     return View(user);
        // }

        // [HttpPost]
        // public async Task<IActionResult> EditUser(User user)
        // {
        //     if (!ModelState.IsValid) return View(user);

        //     bool success = await _userService.UpdateUserAsync(user);
        //     if (success) TempData["SuccessMessage"] = "User updated successfully!";

        //     return RedirectToAction("UsersList");
        // }
#endregion

        // public async Task<IActionResult> DeleteUser(int id)
        // {
        //     bool success = await _userService.DeleteUserAsync(id);
        //     if (success) TempData["SuccessMessage"] = "User deleted successfully!";

        //     return RedirectToAction("UsersList");
        // }
    }
}

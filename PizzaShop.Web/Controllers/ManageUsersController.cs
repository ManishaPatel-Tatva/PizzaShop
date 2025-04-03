using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PizzaShop.Service.Interfaces;
using PizzaShop.Entity.ViewModels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using PizzaShop.Web.Filters;
using PizzaShop.Entity.Models;
using PizzaShop.Service.Common;

namespace PizzaShop.Web.Controllers
{
    [Authorize]
    public class ManageUsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAddressService _addressService; 
        private readonly IJwtService _jwtService;

        public ManageUsersController(IUserService userService, IJwtService jwtService, IAddressService addressService)
        {
            _userService = userService;
            _jwtService = jwtService;
            _addressService = addressService;
        }


#region Display User
/*---------------------------Display Users---------------------------------------------
---------------------------------------------------------------------------------------*/

        [CustomAuthorize("View_Users")]
        public IActionResult Index()
        {
            UsersListViewModel model = new()
            { 
                Users = Enumerable.Empty<UserInfoViewModel>(),
                Page = new Pagination() 
            };
            
            ViewData["sidebar-active"] = "Users";
            return View(model);       
        }
        
        [CustomAuthorize("View_Users")]
        public async Task<IActionResult> GetUsersList(int pageSize = 5, int pageNumber = 1, string search="")
        {
            UsersListViewModel? model = await _userService.GetPagedRecords(pageSize, pageNumber, search);
            if (model == null)
            {
                return NotFound(); // This triggers AJAX error
            }

            return PartialView("_UsersPartialView", model);
        }
#endregion


#region Add user
/*---------------------------Add User---------------------------------------------
---------------------------------------------------------------------------------------*/
        
        [CustomAuthorize("Edit_Users")]
        [HttpGet]
        public async Task<IActionResult> AddUser()
        {
            AddUserViewModel model = await _userService.GetAddUser();
            ViewData["sidebar-active"] = "Users";
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(AddUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                AddUserViewModel addUserModel = await _userService.GetAddUser();
                TempData["errorMessage"] = NotificationMessages.CreatedFailed.Replace("{0}","User");
                ViewData["sidebar-active"] = "Users";
                return View(addUserModel);
            }
                
            string? token = Request.Cookies["authToken"];
            string? createrEmail = _jwtService.GetClaimValue(token, "email");

            (bool isAdded, string message) = await _userService.AddUserAsync(model, createrEmail);
            if (!isAdded)
            {
                AddUserViewModel addUserModel = await _userService.GetAddUser();
                TempData["errorMessage"] = message;
                ViewData["sidebar-active"] = "Users";
                return View(addUserModel);
            }
            
            TempData["successMessage"] = NotificationMessages.Created.Replace("{0}","User");
            return RedirectToAction("Index");
        }
#endregion


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

#region Edit User
/*---------------------------Edit User---------------------------------------------
---------------------------------------------------------------------------------------*/
        [CustomAuthorize("Edit_Users")]
        [HttpGet]
        public async Task<IActionResult> EditUser(long userId)
        {
            EditUserViewModel model =  await _userService.GetUserAsync(userId);
            if (model == null)
            {
                ViewData["sidebar-active"] = "Users";
                return NotFound();
            } 

            ViewData["sidebar-active"] = "Users";
            return View(model);
        }

        [CustomAuthorize("Edit_Users")]
        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                EditUserViewModel editUserModel =  await _userService.GetUserAsync(model.UserId);
                ViewData["sidebar-active"] = "Users";
                return View(editUserModel);
            }

            (bool isUpdated, string message) = await _userService.UpdateUser(model);

            if (!isUpdated)
            {
                EditUserViewModel editUserModel =  await _userService.GetUserAsync(model.UserId);
                TempData["errorMessage"] = message;
                ViewData["sidebar-active"] = "Users";
                return View(editUserModel);
            }

            TempData["successMessage"] = NotificationMessages.Updated.Replace("{0}","User");
            return RedirectToAction("Index","ManageUsers");
        }
#endregion


#region Soft Delete User
/*-------------------------------------Soft Delete User-------------------------------------------------------
-------------------------------------------------------------------------------------------------------*/
        [CustomAuthorize("Delete_Users")]
        [HttpPost]
        public async Task<IActionResult> SoftDeleteUser(long id)
        {
            bool success = await _userService.SoftDeleteUser(id);

            if(!success)
            {
                return Json(new {success = false, message="User Not deleted"});
            }
            return Json(new {success = true, message="User deleted Successfully!"});
        }

#endregion 

    }
}

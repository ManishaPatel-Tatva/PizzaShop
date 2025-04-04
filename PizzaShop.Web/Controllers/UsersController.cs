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
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAddressService _addressService;
        private readonly IJwtService _jwtService;

        public UsersController(IUserService userService, IJwtService jwtService, IAddressService addressService)
        {
            _userService = userService;
            _jwtService = jwtService;
            _addressService = addressService;
        }


        #region Get
        /*---------------------------View Users---------------------------------------------
        ---------------------------------------------------------------------------------------*/
        [CustomAuthorize(nameof(PermissionType.View_Users))]
        public IActionResult Index()
        {
            UserPaginationViewModel model = new()
            {
                Users = Enumerable.Empty<UserInfoViewModel>(),
                Page = new Pagination()
            };

            ViewData["sidebar-active"] = "Users";
            return View(model);
        }

        [HttpPost]
        [CustomAuthorize(nameof(PermissionType.View_Users))]
        public async Task<IActionResult> GetList(FilterViewModel filter)
        {
            UserPaginationViewModel? list = await _userService.Get(filter);
            return PartialView("_UsersListPartialView", list);
        }
        #endregion Get

        #region Add
        /*--------------------------------------------Add User------------------------------------------------------------------------------------
        ------------------------------------------------------------------------------------------------------------------------------------*/
        [HttpGet]
        [CustomAuthorize(nameof(PermissionType.Edit_Users))]
        public async Task<IActionResult> Add()
        {
            AddUserViewModel model = await _userService.Get();
            ViewData["sidebar-active"] = "Users";
            return View(model);
        }

        [HttpPost]
        [CustomAuthorize(nameof(PermissionType.Edit_Users))]
        public async Task<IActionResult> Add(AddUserViewModel model)
        {
            ViewData["sidebar-active"] = "Users";

            if (!ModelState.IsValid)
            {
                AddUserViewModel addUserModel = await _userService.Get();
                return View(addUserModel);
            }

            string? token = Request.Cookies["authToken"];
            string? createrEmail = _jwtService.GetClaimValue(token, "email");

            ResponseViewModel response = await _userService.Add(model, createrEmail);
            if (!response.Success)
            {
                AddUserViewModel addUserModel = await _userService.Get();
                TempData["NotificationMessage"] = response.Message;
                TempData["NotificationType"] = NotificationType.Error.ToString();
                return View(addUserModel);
            }

            TempData["NotificationMessage"] = response.Message;
            TempData["NotificationType"] = NotificationType.Success.ToString();
            return RedirectToAction("Index");
        }
        #endregion

        #region Edit
        /*-----------------------------------------------------------------------------Edit User---------------------------------------------
        ------------------------------------------------------------------------------------------------------------------------------------*/
        [HttpGet]
        [CustomAuthorize(nameof(PermissionType.Edit_Users))]
        public async Task<IActionResult> Edit(long userId)
        {
            ViewData["sidebar-active"] = "Users";
            EditUserViewModel model = await _userService.Get(userId);
            return View(model);
        }

        [HttpPost]
        [CustomAuthorize(nameof(PermissionType.Edit_Users))]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            ViewData["sidebar-active"] = "Users";
            if (!ModelState.IsValid)
            {
                EditUserViewModel editUserModel = await _userService.Get(model.UserId);
                return View(editUserModel);
            }

            string? token = Request.Cookies["authToken"];
            string? createrEmail = _jwtService.GetClaimValue(token, "email");

            ResponseViewModel response = await _userService.Update(model, createrEmail);

            if (!response.Success)
            {
                EditUserViewModel editUserModel = await _userService.Get(model.UserId);
                TempData["NotificationMessage"] = response.Message;
                TempData["NotificationType"] = NotificationType.Error.ToString();
                return View(editUserModel);
            }

            TempData["NotificationMessage"] = response.Message;
            TempData["NotificationType"] = NotificationType.Success.ToString();
            return RedirectToAction("Index", "Users");
        }
        #endregion

        #region Delete User
        /*-------------------------------------Delete User-------------------------------------------------------
        -------------------------------------------------------------------------------------------------------*/
        [CustomAuthorize(nameof(PermissionType.Delete_Users))]
        [HttpPost]
        public async Task<IActionResult> Delete(long id)
        {
            if (!await _userService.Delete(id))
            {
                return Json(new { success = false, message = NotificationMessages.DeletedFailed.Replace("{0}", "User") });
            }
            return Json(new { success = true, message = NotificationMessages.Deleted.Replace("{0}", "User") });
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
    }
}

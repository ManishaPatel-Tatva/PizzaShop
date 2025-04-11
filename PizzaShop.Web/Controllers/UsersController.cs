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
        [HttpGet]
        [CustomAuthorize(nameof(PermissionType.View_Users))]
        public IActionResult Index()
        {
            ViewData["sidebar-active"] = "Users";
            return View();
        }

        [HttpPost]
        [CustomAuthorize(nameof(PermissionType.View_Users))]
        public async Task<IActionResult> GetList(FilterViewModel filter)
        {
            UserPaginationViewModel? list = await _userService.Get(filter);
            return PartialView("_ListPartialView", list);
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

            ResponseViewModel response = await _userService.Add(model);
            TempData["NotificationMessage"] = response.Message;
            if (response.Success)
            {
                TempData["NotificationType"] = NotificationType.Success.ToString();
                return RedirectToAction("Index");
            }
            else
            {
                AddUserViewModel addUserModel = await _userService.Get();
                TempData["NotificationType"] = NotificationType.Error.ToString();
                return View(addUserModel);
            }


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
                EditUserViewModel user = await _userService.Get(model.UserId);
                return View(user);
            }

            ResponseViewModel response = await _userService.Update(model);

            TempData["NotificationMessage"] = response.Message;
            if (response.Success)
            {
                TempData["NotificationType"] = NotificationType.Success.ToString();
                return RedirectToAction("Index", "Users");
            }
            else
            {
                TempData["NotificationType"] = NotificationType.Error.ToString();
                EditUserViewModel user = await _userService.Get(model.UserId);
                return View(user);
            }
        }
        #endregion

        #region Delete
        /*-------------------------------------Delete User-------------------------------------------------------
        -------------------------------------------------------------------------------------------------------*/
        [HttpPost]
        [CustomAuthorize(nameof(PermissionType.Delete_Users))]
        public async Task<IActionResult> Delete(long id)
        {
            ResponseViewModel response = await _userService.Delete(id);
            return Json(response);
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

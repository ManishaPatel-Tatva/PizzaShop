using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Services;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace PizzaShop.Controllers
{
    public class ManageUserController : Controller
    {
        private readonly IUserService _userService;

        public ManageUserController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> UsersList()
        {
            var users = _userService.GetAllUsersAsync();
            return View();
        }

        public IActionResult AddUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(User user)
        {
            if (!ModelState.IsValid) return View(user);

            bool success = await _userService.AddUserAsync(user);
            if (success) TempData["SuccessMessage"] = "User added successfully!";

            return RedirectToAction("UsersList");
        }

        public async Task<IActionResult> EditUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(User user)
        {
            if (!ModelState.IsValid) return View(user);

            bool success = await _userService.UpdateUserAsync(user);
            if (success) TempData["SuccessMessage"] = "User updated successfully!";

            return RedirectToAction("UsersList");
        }

        public async Task<IActionResult> DeleteUser(int id)
        {
            bool success = await _userService.DeleteUserAsync(id);
            if (success) TempData["SuccessMessage"] = "User deleted successfully!";

            return RedirectToAction("UsersList");
        }
    }
}

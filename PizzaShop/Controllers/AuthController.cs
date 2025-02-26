using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Services;
using DataAccessLayer.ViewModels;

namespace PizzaShop.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

/*---------------------------------------------------------Login-----------------------------------------------------------------
--------------------------------------------------------------------------------------------------------------------------------*/
#region Login

        [HttpGet]
        public IActionResult Login()
        {
            if (Request.Cookies["emailCookie"] != null)
                return RedirectToAction("MyProfile", "Profile");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) 
                return View(model);

            var success = await _authService.LoginAsync(model.Email, model.Password, model.RememberMe);

            if (success)
                return RedirectToAction("MyProfile", "Profile");

            return View(model);
        }

#endregion

/*-------------------------------------------------------Forgot Password-----------------------------------------------------------------
--------------------------------------------------------------------------------------------------------------------------------*/
#region Forgot Password

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) 
                return View(model);

            await _authService.ForgotPasswordAsync(model.Email);
            return RedirectToAction("Login","Auth");
        }

#endregion

/*-------------------------------------------------------Reset Password-----------------------------------------------------------------
--------------------------------------------------------------------------------------------------------------------------------*/
#region Reset Password

        [HttpGet]
        public IActionResult ResetPassword(string email)
        {
            ViewBag.email = email;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var success = await _authService.ResetPasswordAsync(model.Email, model.NewPassword, model.ConfirmPassword);
            if (success)
            {
                return RedirectToAction("Login","Auth");
            } 

            return View(model);
        }

#endregion

    }
}


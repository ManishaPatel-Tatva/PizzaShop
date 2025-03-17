using Microsoft.AspNetCore.Mvc;
using PizzaShop.Service.Interfaces;
using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Web.Controllers;

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
            return RedirectToAction("Dashboard", "Profile");

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) 
            return View(model);

        var (token, userName, profileImg, message) = await _authService.LoginAsync(model.Email, model.Password);

        if(token == null)
            TempData["errorMessage"] = message;

        if (token != null)
        {
            CookieOptions options = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(1),
                HttpOnly = true,
                IsEssential = true
            };
            Response.Cookies.Append("authToken", token, options);
            Response.Cookies.Append("userName", userName, options);
            Response.Cookies.Append("profileImg", profileImg, options);

            if (model.RememberMe)
                Response.Cookies.Append("emailCookie", model.Email, options);

            return RedirectToAction("Dashboard", "Profile");
        }

        ModelState.AddModelError("", "Invalid email or password.");
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

        // Generate a secure token for reset password (GUID-based)
        var resetToken = Guid.NewGuid().ToString();
        var resetLink = Url.Action("ResetPassword", "Auth", new { token = resetToken }, Request.Scheme);

        var (success, message) = await _authService.ForgotPasswordAsync(model.Email, resetToken, resetLink);
        if(success)
        {
            TempData["successMessage"] = message;
            return RedirectToAction("Login","Auth");
        }

        TempData["errorMessage"] = message;
        ModelState.AddModelError("", "Email not found.");
        return View(model);
    }

#endregion

/*-------------------------------------------------------Reset Password-----------------------------------------------------------------
--------------------------------------------------------------------------------------------------------------------------------*/
#region Reset Password

    [HttpGet]
    public IActionResult ResetPassword(string token)
    {
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Auth");

        ViewBag.Token = token; 
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var (success, message) = await _authService.ResetPasswordAsync(model.Token, model.NewPassword);
        if (success)
        {
            TempData["successMessage"] = message;
            return RedirectToAction("Login","Auth");
        } 

        TempData["errorMessage"] = message;
        ModelState.AddModelError("", "Failed to reset password.");
        return View(model);
    }

#endregion

}



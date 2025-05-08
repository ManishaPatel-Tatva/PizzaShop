using Microsoft.AspNetCore.Mvc;
using PizzaShop.Service.Interfaces;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Service.Common;

namespace PizzaShop.Web.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    #region Login
    /*---------------------------------------------------------Login-----------------------------------------------------------------
    --------------------------------------------------------------------------------------------------------------------------------*/
    [HttpGet]
    public IActionResult Login()
    {
        if (Request.Cookies["emailCookie"] != null)
        {
            return RedirectToAction("Dashboard", "Profile");
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        (LoginResultViewModel loginResult, ResponseViewModel response) = await _authService.LoginAsync(model.Email, model.Password);

        if (!response.Success)
        {
            TempData["NotificationMessage"] = response.Message;
            TempData["NotificationType"] = NotificationType.Error.ToString();
            return View(model);
        }

        if (loginResult.Token != null)
        {
            CookieOptions options = new()
            {
                Expires = DateTime.Now.AddDays(1),
                HttpOnly = true,
                IsEssential = true,
                Secure = true
            };

            HttpContext.Session.SetString("email", model.Email);

            Response.Cookies.Append("authToken", loginResult.Token, options);
            Response.Cookies.Append("userName", loginResult.UserName, options);
            Response.Cookies.Append("profileImg", loginResult.ImageUrl, options);

            if (model.RememberMe)
            {
                Response.Cookies.Append("emailCookie", model.Email, options);
            }

            return RedirectToAction("Dashboard", "Profile");
        }

        TempData["NotificationMessage"] = response.Message;
        TempData["NotificationType"] = NotificationType.Error.ToString();
        return View(model);
    }

    #endregion

    #region Forgot Password
    /*-------------------------------------------------------Forgot Password-----------------------------------------------------------------
    --------------------------------------------------------------------------------------------------------------------------------*/
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

        ResponseViewModel response = await _authService.ForgotPassword(model.Email, resetToken, resetLink);
        if (!response.Success)
        {
            TempData["NotificationMessage"] = response.Message;
            TempData["NotificationType"] = NotificationType.Error.ToString();
            return View(model);
        }

        TempData["NotificationMessage"] = response.Message;
        TempData["NotificationType"] = NotificationType.Success.ToString();
        return RedirectToAction("Login", "Auth");
    }

    #endregion

    #region Reset Password
    /*-------------------------------------------------------Reset Password-----------------------------------------------------------------
    --------------------------------------------------------------------------------------------------------------------------------*/
    [HttpGet]
    public IActionResult ResetPassword(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Auth");
        }

        ViewBag.Token = token;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        ResponseViewModel response = await _authService.ResetPassword(model.Token, model.NewPassword);
        if (!response.Success)
        {
            TempData["NotificationMessage"] = response.Message;
            TempData["NotificationType"] = NotificationType.Error.ToString();
            return View(model);
        }

        TempData["NotificationMessage"] = response.Message;
        TempData["NotificationType"] = NotificationType.Success.ToString();
        return RedirectToAction("Login", "Auth");
    }

    #endregion

    #region Error

    [Route("/Auth/Error/{code}")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(int code)
    {
        if (code == 404)
        {
            return View("404");
        }
        if (code == 401)
        {
            return View("401");
        }
        if (code == 403)
        {
            return View("403");
        }
       
        return View("500");
    }

    public IActionResult HandleErrorWithToast(string message)
    {
        TempData["ToastError"] = message;

        string referer = Request.Headers["Referer"].ToString();

        if (string.IsNullOrEmpty(referer))
        {
            referer = Url.Action("Login", "Auth") ?? "/"; // or any safe fallback page
        }

        return Redirect(referer);
    }

    #endregion

}



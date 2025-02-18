﻿using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PizzaShop.Models;
using PizzaShop.Services;
using PizzaShop.ViewModel;

namespace PizzaShop.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly PizzashopContext _context;
    private readonly IEmailService _emailService;

    public HomeController(ILogger<HomeController> logger, PizzashopContext context, IEmailService emailService)
    {
        _logger = logger;
        _context = context;
        _emailService = emailService;
    }


    public IActionResult Login()
    {
        if(Request.Cookies["emailCookie"] != null)
        {
             return RedirectToAction("Privacy");
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        CookieOptions options = new CookieOptions();
        options.Expires = DateTime.Now.AddDays(15);

        if(ModelState.IsValid){
            var users = await _context.Users.Where(u => u.Email == model.Email).Select(x=> new{x.Email, x.Password}).FirstOrDefaultAsync();

            if(users != null && users.Password == model.Password){

                //For cookies               
                if(model.RememberMe)
                {
                    Response.Cookies.Append("emailCookie",model.Email,options);
                }

                return RedirectToAction("Privacy");
            }
        
        }
        return View(model);

    }

    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        var resetLink = Url.Action("ResetPassword","Home", new{email = model.Email},Request.Scheme);
        string body = $@"<div style='background-color: #F2F2F2;'>
        <div style='background-color: #0066A8; color: white; height: 90px; font-size: 40px; font-weight: 600; text-align: center; padding-top: 40px; margin-bottom: 0px;'>PIZZASHOP</div>
        <div style='font-family:Verdana, Geneva, Tahoma, sans-serif; margin-top: 0px; font-size: 20px; padding: 10px;'>
            <p>Pizza shop,</p>
            <p>Please click <a href='{resetLink}'>here</a> for reset your account Password.</p>
            <p>If you encounter any issues or have any question, please do not hesitate to contact our support team.</p>
            <p><span style='color: orange;'>Important Note:</span> For security reasons, the link will expire in 24 hours. If you did not request a password reset, please ignore this email or contact our support team immediately.</p>
        </div>
    </div>";

        if(ModelState.IsValid){
            await _emailService.SendEmailAsync(model.Email, "Reset Password", body);
            return RedirectToAction("Privacy");
        }
        return View();
    }

    public IActionResult ResetPassword()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
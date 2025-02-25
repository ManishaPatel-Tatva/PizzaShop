﻿using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.Models;
using BusinessLogicLayer.Helper;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.ViewModel;
namespace PizzaShop.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    // private readonly PizzaShopContext _context;
    private readonly IEmailService _emailService;
    private readonly JwtService _jwtService;

    public HomeController(ILogger<HomeController> logger, PizzaShopContext context, IEmailService emailService, JwtService jwtService)
    {
        _logger = logger;
        // _context = context;
        _emailService = emailService;
        _jwtService =jwtService;
    }

    public IActionResult Login()
    {
        if(Request.Cookies["emailCookie"] != null)
        {
             return RedirectToAction("MyProfile","Dashboard");
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

        CookieOptions options = new CookieOptions
        {
            Expires = DateTime.Now.AddDays(1), // Cookie expires in 1 days                      
            HttpOnly = true, // Secure against JavaScript access
            IsEssential = true
        };

        // var user = await _context.Users.Where(u => u.Email == model.Email).Select(x=> new{x.Email, x.Password, x.RoleId}).FirstOrDefaultAsync();   //dal   
        // If "Remember Me" is checked, store in a cookie
        if (model.RememberMe)
        {                   
            Response.Cookies.Append("emailCookie", model.Email, options);
        }
        
        bool verified = BCrypt.Net.BCrypt.Verify(model.Password, user.Password);  // bll 
        if(user != null && verified){
            var role = _context.Roles.Where(u => u.Id == user.RoleId).FirstOrDefault(); //dal
            var token = _jwtService.GenerateToken(model.Email,role.Name);   //bll

            Response.Cookies.Append("authToken",token, options);
            return RedirectToAction("MyProfile","Dashboard");
        }
        else{
            return View(model);
        }
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

    public IActionResult ResetPassword(string email)
    {
        ViewBag.email = email;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model){

        if(!ModelState.IsValid){
            return RedirectToAction("Privacy");
        }

        var user = await _context.Users.Where(u => u.Email == model.Email).FirstOrDefaultAsync();
        if(user == null)
        {
            ModelState.AddModelError("","Email not found");
        }

        if(model.NewPassword == model.ConfirmPassword)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(model.ConfirmPassword);
            user.Password = passwordHash;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Login");
        }
        else{
            return View(model);
        }
    }

    [Authorize(Roles = "admin")]
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
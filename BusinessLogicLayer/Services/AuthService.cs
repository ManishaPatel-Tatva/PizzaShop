using System;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Http;
using BCrypt.Net;
using DataAccessLayer.Interfaces;
using BusinessLogicLayer.Helpers;

namespace BusinessLogicLayer.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly JwtService _jwtService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(IUserRepository userRepository, IEmailService emailService, JwtService jwtService, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _jwtService = jwtService;
            _httpContextAccessor = httpContextAccessor;
        }

/*-----------------------------------------------------------------Login---------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------------------*/
#region Login
        public async Task<bool> LoginAsync(string email, string password, bool rememberMe)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
                return false;

            var role = await _userRepository.GetUserRoleAsync(user.RoleId);
            var token = _jwtService.GenerateToken(email,role.Name);

            CookieOptions options = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(1),
                HttpOnly = true,
                IsEssential = true
            };

            _httpContextAccessor.HttpContext.Response.Cookies.Append("authToken", token, options);
            if (rememberMe)
                _httpContextAccessor.HttpContext.Response.Cookies.Append("emailCookie", email, options);

            return true;
        }
#endregion

/*---------------------------------------------------------------Forgot Password------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------------------*/
#region ForgotPassword
        public async Task<bool> ForgotPasswordAsync(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null) return false;

            var resetLink = $"https://localhost:5001/Home/ResetPassword?email={email}";
            // var resetLink = Url.Action("ResetPassword","Home", new{email = model.Email},Request.Scheme);

            string body = $@"
                <div style='background-color: #F2F2F2;'>
                    <div style='background-color: #0066A8; color: white; height: 90px; font-size: 40px; font-weight: 600; text-align: center; padding-top: 40px; margin-bottom: 0px;'>PIZZASHOP</div>
                    <div style='font-family:Verdana, Geneva, Tahoma, sans-serif; margin-top: 0px; font-size: 20px; padding: 10px;'>
                        <p>Pizza shop,</p>
                        <p>Please click <a href='{resetLink}'>here</a> for reset your account Password.</p>
                        <p>If you encounter any issues or have any question, please do not hesitate to contact our support team.</p>
                        <p><span style='color: orange;'>Important Note:</span> For security reasons, the link will expire in 24 hours. If you did not request a password reset, please ignore this email or contact our support team immediately.</p>
                    </div>
                </div>";

            await _emailService.SendEmailAsync(email, "Reset Password", body);
            return true;
        }
#endregion

/*---------------------------------------------------------------Reset Password------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------------------*/
#region ResetPassword
        public async Task<bool> ResetPasswordAsync(string email, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword) 
                return false;

            var user = await _userRepository.GetUserByEmailAsync(email);
            
            if (user == null) 
                return false;

            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _userRepository.UpdateAsync(user);
            return true;
        }
#endregion

    }
}


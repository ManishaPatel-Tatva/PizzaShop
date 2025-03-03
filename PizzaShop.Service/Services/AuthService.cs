using PizzaShop.Service.Interfaces;
using PizzaShop.Service.Helpers;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Entity.Models;
using BusinessLogicLayer.Helpers;

namespace PizzaShop.Service.Services;

public class AuthService : IAuthService
{
    private readonly IGenericRepository<User> _userRepository;
    private readonly IGenericRepository<Role> _roleRepository;
    private readonly IEmailService _emailService;
    private readonly IJwtService _jwtService;

    public AuthService(IGenericRepository<User> userRepository, IGenericRepository<Role> roleRepository, IEmailService emailService, IJwtService jwtService)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _emailService = emailService;
        _jwtService = jwtService;
    }

#region Login
/*--------------------------------Login-------------------------------------------------------------
-----------------------------------------------------------------------------------------------*/
    public async Task<string?> LoginAsync(string email, string password)
    {
        var user = await _userRepository.GetByStringAsync(u => u.Email == email);       //Get User from email

        if (user == null || !PasswordHelper.VerifyPassword(password, user.Password))
            return null;

        var role = await _roleRepository.GetByStringAsync(u => u.Id == user.Id);

        return _jwtService.GenerateToken(email, role.Name);
    }

#endregion

#region ForgotPassword
/*--------------------------------Forgot Password-------------------------------------------------------------
-----------------------------------------------------------------------------------------------*/
    public async Task<bool> ForgotPasswordAsync(string email, string resetLink)
    {
        var user = await _userRepository.GetByStringAsync(u => u.Email == email);

        if (user == null) return false;

        try
        {
            var resetToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray()); // Secure token
            user.Resettoken = resetToken;
            user.Resettokenexpiry = DateTime.UtcNow.AddHours(1); // Token expires in 1 hour
            await _userRepository.UpdateAsync(user);

            var body = EmailTemplateHelper.GetResetPasswordEmail(resetToken);
            await _emailService.SendEmailAsync(email, "Reset Password", body);
            return true;
        }
        catch (Exception)
        {
            return false;
        } 
    }

#endregion

#region ResetPassword
/*--------------------------------Reset Password-------------------------------------------------------------
-----------------------------------------------------------------------------------------------*/
    public async Task<bool> ResetPasswordAsync(string token, string newPassword, string confirmPassword)
    {
        if (newPassword != confirmPassword) 
            return false;

        var user = await _userRepository.GetByStringAsync(u => u.Resettoken == token && u.Resettokenexpiry > DateTime.UtcNow);
        if (user == null) 
            return false;

        user.Password = PasswordHelper.HashPassword(newPassword);
        await _userRepository.UpdateAsync(user);

         // Update the user password and remove/reset the token
        user.Resettoken = null; 
        user.Resettokenexpiry = null; 
        
        return true;
    }

#endregion
}


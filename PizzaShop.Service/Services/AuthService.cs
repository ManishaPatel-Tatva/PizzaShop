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
    private readonly IGenericRepository<ResetPasswordToken> _resetPasswordRepository;

    private readonly IEmailService _emailService;
    private readonly IJwtService _jwtService;

    public AuthService(IGenericRepository<User> userRepository, IGenericRepository<Role> roleRepository, IEmailService emailService, IJwtService jwtService, IGenericRepository<ResetPasswordToken> resetPasswordRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _emailService = emailService;
        _jwtService = jwtService;
        _resetPasswordRepository = resetPasswordRepository;
    }

    #region Login
    /*--------------------------------Login-------------------------------------------------------------
    -----------------------------------------------------------------------------------------------*/

    public async Task<(string? token, string userName, string imageUrl, string? message)> LoginAsync(string email, string password)
    {
        var user = await _userRepository.GetByStringAsync(u => u.Email == email);       //Get User from email

        if (user == null)
            return (null,null,null, "User Doesn't Exist");

        if (!PasswordHelper.VerifyPassword(password, user.Password))
            return (null,null,null, "Invalid Credentials!");

        var role = await _roleRepository.GetByStringAsync(u => u.Id == user.Id);
        var token = _jwtService.GenerateToken(email, role.Name);
        return (token,user.Username, user.ProfileImg, null);
    }

#endregion

#region ForgotPassword
/*--------------------------------Forgot Password-------------------------------------------------------------
-----------------------------------------------------------------------------------------------*/
    public async Task<(bool success, string? message)> ForgotPasswordAsync(string email, string resetToken, string resetLink)
    {
        var user = await _userRepository.GetByStringAsync(u => u.Email == email);

        if (user == null) 
            return (false,"User Doesn't Exist");

        //Stores the reset password token in table  
        ResetPasswordToken token = new ResetPasswordToken
        {
            Email = email,
            Token = resetToken
        };

        var success =  await _resetPasswordRepository.AddAsync(token);

        if(success)
        {
            var body = EmailTemplateHelper.GetResetPasswordEmail(resetLink);
            await _emailService.SendEmailAsync(email, "Reset Password", body);
            return(success, "Email Sent Successfully!");
        }
        
        return (success, "Invalid!");
    }

#endregion

#region ResetPassword
/*--------------------------------Reset Password-------------------------------------------------------------
-----------------------------------------------------------------------------------------------*/
    public async Task<(bool success, string? message)> ResetPasswordAsync(string token, string newPassword)
    {
        var resetToken = await _resetPasswordRepository.GetByStringAsync(u => u.Token == token);
        if (resetToken == null) 
            return (false, "Invalid URL");

        var difference = resetToken.Expirytime.Subtract(DateTime.Now).Ticks;
        if(difference <= 0)
            return (false, "Link Expired");    

        if (resetToken.IsUsed)
            return (false, " You already used this link to reset password");

        var user = await _userRepository.GetByStringAsync(u => u.Email == resetToken.Email); 

        user.Password = PasswordHelper.HashPassword(newPassword);
        var success = await _userRepository.UpdateAsync(user);

        if(success)
        {
            resetToken.IsUsed = true;
            return(true, "Password Changed Successfully!");
        }
        
        return (success, "Reset Password Failed!");
    }

#endregion
}


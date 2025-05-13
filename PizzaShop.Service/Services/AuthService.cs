using PizzaShop.Service.Interfaces;
using PizzaShop.Service.Helpers;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Entity.Models;
using PizzaShop.Service.Common;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Service.Exceptions;

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
    public async Task<(LoginResultViewModel loginResult, ResponseViewModel response)> LoginAsync(string email, string password)
    {
        User user = await _userRepository.GetByStringAsync(u => u.Email == email && !u.IsDeleted && u.IsActive == true) 
                    ?? throw new NotFoundException(NotificationMessages.NotFound.Replace("{0}", "User"));       //Get User from email

        LoginResultViewModel loginVM = new();
        ResponseViewModel response = new();

        if (!PasswordHelper.VerifyPassword(password, user.Password))
        {
            response.Success = false;
            response.Message = NotificationMessages.Invalid.Replace("{0}", "Credentials");
            return (loginVM, response);
        }

        Role role = await _roleRepository.GetByStringAsync(u => u.Id == user.RoleId) 
                    ?? throw new NotFoundException(NotificationMessages.NotFound.Replace("{0}", "Role"));
        string token = await _jwtService.GenerateToken(email, role.Name);

        LoginResultViewModel loginResult = new()
        {
            Token = token,
            UserName = user.Username,
            ImageUrl = user.ProfileImg
        };

        response.Success = true;
        return (loginResult, response);
    }
    #endregion

    #region ForgotPassword
    /*--------------------------------Forgot Password-------------------------------------------------------------
    -----------------------------------------------------------------------------------------------*/
    public async Task<ResponseViewModel> ForgotPassword(string email, string resetToken, string resetLink)
    {
        User user = await _userRepository.GetByStringAsync(u => u.Email == email && !u.IsDeleted) 
                    ?? throw new NotFoundException(NotificationMessages.NotFound.Replace("{0}", "User"));

        ResponseViewModel response = new();

        //Stores the reset password token in table  
        ResetPasswordToken token = new()
        {
            Email = email,
            Token = resetToken
        };

        await _resetPasswordRepository.AddAsync(token);

        //Sending email to user for resetting password
        string body = EmailTemplateHelper.ResetPassword(resetLink);
        if (await _emailService.SendEmail(email, "Reset Password", body))
        {
            response.Success = true;
            response.Message = NotificationMessages.EmailSent;
        }
        else
        {
            response.Success = false;
            response.Message = NotificationMessages.EmailSendingFailed;
        }

        return response;
    }

    #endregion

    #region ResetPassword
    /*--------------------------------Reset Password-------------------------------------------------------------
    -----------------------------------------------------------------------------------------------*/
    public async Task<ResponseViewModel> ResetPassword(string token, string newPassword)
    {
        ResetPasswordToken? resetToken = await _resetPasswordRepository.GetByStringAsync(t => t.Token == token) 
                                        ?? throw new NotFoundException(NotificationMessages.Invalid.Replace("{0}", "Token"));

        ResponseViewModel response = new();

        //Check token expiry
        if (resetToken.Expirytime.Subtract(DateTime.Now).Ticks <= 0)
        {
            response.Success = false;
            response.Message = NotificationMessages.LinkExpired;
            return response;
        }

        //Check if token is already used
        if (resetToken.IsUsed)
        {
            response.Success = false;
            response.Message = NotificationMessages.AlreadyUsed;
        }

        User user = await _userRepository.GetByStringAsync(u => u.Email == resetToken.Email) 
                    ?? throw new NotFoundException(NotificationMessages.NotFound.Replace("{0}", "User"));

        // Updates Password
        user.Password = PasswordHelper.HashPassword(newPassword);
        await _userRepository.UpdateAsync(user);

        // CHange token status to used
        resetToken.IsUsed = true;
        await _resetPasswordRepository.UpdateAsync(resetToken);

        response.Success = true;
        response.Message = NotificationMessages.Updated.Replace("{0}", "Password");
        return response;
    }

    #endregion
}


using PizzaShop.Service.Interfaces;
using PizzaShop.Service.Helpers;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Entity.Models;
using PizzaShop.Service.Common;
using PizzaShop.Entity.ViewModels;

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
        User? user = await _userRepository.GetByStringAsync(u => u.Email == email);       //Get User from email

        if (user == null)
        {
            return (new LoginResultViewModel { }, new ResponseViewModel
            {
                Success = false,
                Message = NotificationMessages.NotFound.Replace("{0}", "User")
            });
        }

        if (!PasswordHelper.VerifyPassword(password, user.Password))
        {
            return (new LoginResultViewModel{}, new ResponseViewModel
            {
                Success = false,
                Message = NotificationMessages.Invalid.Replace("{0}", "Credentials")
            });
        }

        Role? role = await _roleRepository.GetByStringAsync(u => u.Id == user.RoleId);
        string? token = await _jwtService.GenerateToken(email, role.Name);

        LoginResultViewModel loginResult = new()
        {
            Token = token,
            UserName = user.Username,
            ImageUrl = user.ProfileImg
        };

        ResponseViewModel response = new()
        {
            Success = true,
        };

        return (loginResult, response);
    }
    #endregion

    #region ForgotPassword
    /*--------------------------------Forgot Password-------------------------------------------------------------
    -----------------------------------------------------------------------------------------------*/
    public async Task<ResponseViewModel> ForgotPasswordAsync(string email, string resetToken, string resetLink)
    {
        User? user = await _userRepository.GetByStringAsync(u => u.Email == email && !u.IsDeleted);

        if (user == null)
        {
            return new ResponseViewModel
            {
                Success = false,
                Message = NotificationMessages.NotFound.Replace("{0}", "User")
            };
        }

        //Stores the reset password token in table  
        ResetPasswordToken token = new()
        {
            Email = email,
            Token = resetToken
        };

        if (!await _resetPasswordRepository.AddAsync(token))
        {
            return new ResponseViewModel
            {
                Success = false,
                Message = NotificationMessages.TryAgain
            };
        }

        //Sending email to user for resetting password
        string body = EmailTemplateHelper.ResetPassword(resetLink);
        if (!await _emailService.SendEmailAsync(email, "Reset Password", body))
        {
            return new ResponseViewModel
            {
                Success = false,
                Message = NotificationMessages.EmailSendingFailed
            };
        }

        return new ResponseViewModel
        {
            Success = false,
            Message = NotificationMessages.EmailSent
        };
    }

    #endregion

    #region ResetPassword
    /*--------------------------------Reset Password-------------------------------------------------------------
    -----------------------------------------------------------------------------------------------*/
    public async Task<ResponseViewModel> ResetPasswordAsync(string token, string newPassword)
    {
        ResetPasswordToken? resetToken = await _resetPasswordRepository.GetByStringAsync(t => t.Token == token);
        if (resetToken == null)
        {
            return new ResponseViewModel
            {
                Success = false,
                Message = NotificationMessages.Invalid.Replace("{0}", "Token")
            };
        }

        //Check token expiry
        if (resetToken.Expirytime.Subtract(DateTime.Now).Ticks <= 0)
        {
            return new ResponseViewModel
            {
                Success = false,
                Message = NotificationMessages.LinkExpired
            };
        }

        //Check if token is already used
        if (resetToken.IsUsed)
        {
            return new ResponseViewModel
            {
                Success = false,
                Message = NotificationMessages.AlreadyUsed
            };
        }

        User? user = await _userRepository.GetByStringAsync(u => u.Email == resetToken.Email);

        user.Password = PasswordHelper.HashPassword(newPassword);

        if (!await _userRepository.UpdateAsync(user))
        {
            return new ResponseViewModel
            {
                Success = false,
                Message = NotificationMessages.PasswordChangeFailed
            };
        }

        resetToken.IsUsed = true;
        await _resetPasswordRepository.UpdateAsync(resetToken);
        
        return new ResponseViewModel
        {
            Success = true,
            Message = NotificationMessages.PasswordChanged
        };
    }

    #endregion
}


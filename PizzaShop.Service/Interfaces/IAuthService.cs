using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IAuthService
{
    Task<(LoginResultViewModel loginResult, ResponseViewModel response)> LoginAsync(string email, string password);
    Task<ResponseViewModel> ForgotPassword(string email, string resetToken, string resetLink);
    Task<ResponseViewModel> ResetPassword(string token, string newPassword);
}


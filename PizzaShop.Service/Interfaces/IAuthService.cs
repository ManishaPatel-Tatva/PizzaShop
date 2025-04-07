using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IAuthService
{
    Task<(LoginResultViewModel loginResult, ResponseViewModel response)> LoginAsync(string email, string password);
    Task<ResponseViewModel> ForgotPasswordAsync(string email, string resetToken, string resetLink);
    Task<ResponseViewModel> ResetPasswordAsync(string token, string newPassword);
}


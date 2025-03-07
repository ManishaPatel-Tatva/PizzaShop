namespace PizzaShop.Service.Interfaces;

public interface IAuthService
{
    Task<(string? token, string userName, string imageUrl, string? message)> LoginAsync(string email, string password);
    Task<(bool success, string? message)> ForgotPasswordAsync(string email, string resetToken, string resetLink);
    Task<(bool success, string? message)> ResetPasswordAsync(string token, string newPassword);
}


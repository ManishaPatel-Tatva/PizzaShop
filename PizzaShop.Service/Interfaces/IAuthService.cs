namespace PizzaShop.Service.Interfaces;

public interface IAuthService
{
    Task<string?> LoginAsync(string email, string password);
    Task<bool> ForgotPasswordAsync(string email, string resetLink);
    Task<bool> ResetPasswordAsync(string email, string newPassword, string confirmPassword);
}


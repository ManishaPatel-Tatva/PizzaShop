namespace BusinessLogicLayer.Interfaces
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(string email, string password, bool rememberMe);
        Task<bool> ForgotPasswordAsync(string email, string resetLink);
        Task<bool> ResetPasswordAsync(string email, string newPassword, string confirmPassword);
    }
}
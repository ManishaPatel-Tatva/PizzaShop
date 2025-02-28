namespace BusinessLogicLayer.Helpers;

public class PasswordHelper
{
    public string EncryptPassword(string password)
    {
        var hashPassword = BCrypt.Net.BCrypt.HashPassword(password);
        return hashPassword;
    }

}

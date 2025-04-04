namespace PizzaShop.Service.Common;

public static class NotificationMessages
{
    //Generic CRUD Success
    public const string Created = "{0} has been added successfully!";
    public const string Updated = "{0} has been updated successfully!";
    public const string Deleted = "{0} has been deleted successfully!";

    //Generic CRUD Failed
    public const string CreatedFailed = "Failed Adding {0}";
    public const string UpdatedFailed = "Failed Updating {0}";
    public const string DeletedFailed = "Failed Deleting {0}";

    //Login
    public const string LoginSuccess = "You have successfully logged in.";
    public const string InvalidCredentials = "Invalid credentials. Please try again.";
    public const string EmailSendingFailed = "Failed to send the email. Please try again.";
    public const string PasswordChangeFailed = "Failed to change the password. Please try again.";
   
    //Profile
    public const string ProfileUpdated = "Your profile has been updated successfully!";
    public const string EmailSentSuccessfully = "Email has been sent successfully!";
    public const string PasswordChanged = "Your password has been changed successfully.";


    // Error Messages
    public const string InvalidModelState = "Model State Is Invalid!";
}

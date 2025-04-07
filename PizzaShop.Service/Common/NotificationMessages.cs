namespace PizzaShop.Service.Common;

public static class NotificationMessages
{
    //Generic CRUD Success
    public const string Added = "{0} has been added successfully!";
    public const string Updated = "{0} has been updated successfully!";
    public const string Deleted = "{0} has been deleted successfully!";

    //Generic CRUD Failed
    public const string AddedFailed = "Failed Adding {0}";
    public const string UpdatedFailed = "Failed Updating {0}";
    public const string DeletedFailed = "Failed Deleting {0}";

    //Custom Messages
    public const string AlreadyExisted = "{0} already existed!";
    public const string NotFound = "{0} not found!";


    //Login
    public const string Invalid = "Invalid {0}. Please try again.";
    public const string TryAgain = "Please try again!";
    public const string LinkExpired = "Link expired! Please try again!";
    public const string AlreadyUsed = "Link already used to reset password!";
    public const string PasswordChangeFailed = "Failed to change the password. Please try again.";
    public const string PasswordChanged = "Your password has been changed successfully.";
    public const string EmailSent = "Email has been sent successfully!";
    public const string EmailSendingFailed = "Failed to send the email. Please try again.";
    



   
    //Profile
    public const string ProfileUpdated = "Your profile has been updated successfully!";


    // Error Messages
    public const string InvalidModelState = "Model State Is Invalid!";


}

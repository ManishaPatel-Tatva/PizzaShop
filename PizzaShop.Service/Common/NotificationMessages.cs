namespace PizzaShop.Service.Common;

public static class NotificationMessages
{
    //Generic CRUD Success
    public const string Added = "{0} has been added successfully!";
    public const string Updated = "{0} has been updated successfully!";
    public const string Deleted = "{0} has been deleted successfully!";

    //Generic CRUD Failed
    public const string AddedFailed = "Failed Adding {0}!" + TryAgain;
    public const string UpdatedFailed = "Failed Updating {0}!"+ TryAgain;
    public const string DeletedFailed = "Failed Deleting {0}!"+ TryAgain;

    //Generic Messages
    public const string TryAgain = "Please try again!";
    public const string AlreadyExisted = "{0} already existed!" + TryAgain;
    public const string NotFound = "{0} not found!" + TryAgain;
    public const string Invalid = "Invalid {0}!" + TryAgain ;
    public const string AtleastOne = "Select atleast one {0}!" + TryAgain;
    public const string Successfully = "{0} Sucessfully!";
    public const string Failed = "{0} Failed!";



    //Login
    public const string LinkExpired = "Link expired!" + TryAgain;
    public const string AlreadyUsed = "Link already used to reset password!";

    //Email
    public const string EmailSent = "Email has been sent successfully!";
    public const string EmailSendingFailed = "Failed to send the email." + TryAgain;

    //Save order
    public const string CompleteOrderFailed = "All items must be served before completing the order";

    // Assign Table
    public const string CapacityExceeded = "Members should be less than table capacity";


}

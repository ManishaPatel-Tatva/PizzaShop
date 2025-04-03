namespace PizzaShop.Service.Helpers;

public static class EmailTemplateHelper
{
    public static string EmailTemplate = $@"
                <div style='background-color: #F2F2F2;'>
                    <div style='background-color: #0066A8; color: white; height: 90px; font-size: 40px; font-weight: 600; text-align: center; padding-top: 40px; margin-bottom: 0px;'>PIZZASHOP</div>
                    <div style='font-family:Verdana, Geneva, Tahoma, sans-serif; margin-top: 0px; font-size: 20px; padding: 10px;'>
                        {0}
                    </div>
                </div>";
    public static string GetResetPasswordEmail(string resetLink)
    {
        string body = $@"<p>Pizza shop,</p>
                        <p>Please click <a href='{resetLink}'>here</a> to reset your account password.</p>
                        <p>If you have any issues, please contact support.</p>
                        <p><span style='color: orange;'>Important:</span> The link expires in 24 hours.</p>";
        return EmailTemplate.Replace("{0}", body);
    }

    public static string GetNewPasswordEmail(string password)
    {
        string body = $@"<p>Pizza shop,</p>
                        <h3>Your Password is : {password}</h3>
                        <p>If you encounter any issues or have any question, please do not hesitate to contact our support team.</p>";
        return EmailTemplate.Replace("{0}", body);
    }
}


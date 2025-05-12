using Microsoft.Extensions.Configuration;

namespace PizzaShop.Service.Configuration;

public static class EmailConfig
{
    public static string Host { get; set; } = "mail.etatvasoft.com";
    public static int Port { get; set; } = 587; 
    public static string UserName { get; set; } = "test.dotnet@etatvasoft.com";
    public static string Password { get; set; } = "P}N^{z-]7Ilp";
    public static string FromEmail { get; set; } = "test.dotnet@etatvasoft.com";
    public static string FromName { get; set; } = "PizzaShop";

    public static void LoadEmailConfiguration(IConfiguration configuration)
    {
        Host = configuration["Host"] ?? Host;
        UserName = configuration["UserName"] ?? UserName;
        Password = configuration["Password"] ?? Password;
        FromEmail = configuration["FromEmail"] ?? FromEmail;

        if(int.TryParse(configuration["Port"], out int port))
        {
            Port = port;
        }

    }
}


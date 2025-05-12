using Microsoft.Extensions.Configuration;

namespace PizzaShop.Service.Configuration;

public static class JwtConfig
{
    public static string Key { get; set; } = "2a43b3035e77fe84972c374c703d3b6a0ae70312b4d644b25204df5998b7b5b998ab92f501d6a62a8c0b8d1660678980244b9cf34614c1dbdf1344378715d9b2eb80fd86674462b3051a5aaeb744711056d3c55f4aaef5162213707be2e0de09a9733c0a12abd619d6e189d98df4218efa6de148b063ec48d002cde08ac34bd7";
    public static string Issuer { get; set; } = "localhost";
    public static string Audience { get; set; } = "localhost";
    public static int TokenDuration { get; set; } = 24;

    public static void LoadJwtConfiguration(IConfiguration configuration)
    {
        Key = configuration["JwtConfig:Key"] ?? Key;
        Issuer = configuration["JwtConfig:Issuer"] ?? Issuer;
        Audience = configuration["JwtConfig:Audience"] ?? Audience;

        if (int.TryParse(configuration["JwtConfig:TokenDuration"], out int duration))
        {
            TokenDuration = duration;
        }
    }
}
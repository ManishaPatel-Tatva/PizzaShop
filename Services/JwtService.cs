using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace PizzaShop.Services;

public class JwtService
{
    private readonly string _secretKey;
    private readonly int _tokenDuration;

    public JwtService(IConfiguration configuration)
    {
        _secretKey = configuration.GetValue<string>("JwtConfig:Key");
        _tokenDuration = configuration.GetValue<int>("JwtConfig:Duration");
    }

    public string GenerateToken(string email, string role)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("email", email),
            new Claim("role", role)   
        };

        var token = new JwtSecurityToken(
            issuer: "localhost",
            audience: "localhost",
            claims: claims,
            expires: DateTime.Now.AddHours(_tokenDuration),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

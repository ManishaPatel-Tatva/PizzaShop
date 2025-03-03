using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Services;

public class JwtService : IJwtService
{
    private readonly string _secretKey;
    private readonly int _tokenDuration;

    public JwtService(IConfiguration configuration)
    {
        _secretKey = configuration["JwtConfig:Key"];
        _tokenDuration = int.Parse(configuration["JwtConfig:Duration"] ?? "24"); // Default: 24 hours
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

    // Extracts claims from a JWT token.
    public ClaimsPrincipal? GetClaimsFromToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var claims = new ClaimsIdentity(jwtToken.Claims);
        return new ClaimsPrincipal(claims);
    }

    // Retrieves a specific claim value from a JWT token.
    public string? GetClaimValue(string token, string claimType)
    {
        var claimsPrincipal = GetClaimsFromToken(token);
        var value = claimsPrincipal?.FindFirst(claimType)?.Value;
        return value;
    }

}

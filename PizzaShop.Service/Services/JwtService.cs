using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PizzaShop.Entity.Models;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Common;
using PizzaShop.Service.Configuration;
using PizzaShop.Service.Exceptions;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Services;

public class JwtService : IJwtService
{
    private readonly IGenericRepository<User> _userRepository ;

    public JwtService(IGenericRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<string> GenerateToken(string email, string role)
    {
        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(JwtConfig.Key));
        SigningCredentials? credentials = new(key, SecurityAlgorithms.HmacSha256);

        User user = await _userRepository.GetByStringAsync(u => u.Email == email) ?? throw new NotFoundException(NotificationMessages.NotFound.Replace("{0}","User"));

        List<Claim>? claims = new()
        {
            new("email", email),
            new("role", role),
            new("roleId", user.RoleId.ToString()),
        };

        JwtSecurityToken? token = new(
            issuer: JwtConfig.Issuer,
            audience: JwtConfig.Audience,
            claims: claims,
            expires: DateTime.Now.AddHours(JwtConfig.TokenDuration),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // Extracts claims from a JWT token.
    public ClaimsPrincipal? GetClaimsFromToken(string token)
    {
        JwtSecurityTokenHandler? handler = new();
        JwtSecurityToken? jwtToken = handler.ReadJwtToken(token);
        ClaimsIdentity? claims = new(jwtToken.Claims);
        return new ClaimsPrincipal(claims);
    }

    // Retrieves a specific claim value from a JWT token.
    public string? GetClaimValue(string token, string claimType)
    {
        ClaimsPrincipal? claimsPrincipal = GetClaimsFromToken(token);
        string? value = claimsPrincipal?.FindFirst(claimType)?.Value;
        return value;
    }

}

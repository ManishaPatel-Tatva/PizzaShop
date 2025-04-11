using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Services;

public class JwtService : IJwtService
{
    private readonly string _secretKey;
    private readonly int _tokenDuration;
    private readonly IRolePermissionService _rolePermissionService ;
    private readonly IGenericRepository<User> _userRepository ;

    public JwtService(IConfiguration configuration, IRolePermissionService rolePermissionService, IGenericRepository<User> userRepository)
    {
        _secretKey = configuration["JwtConfig:Key"];
        _tokenDuration = int.Parse(configuration["JwtConfig:Duration"] ?? "24"); // Default: 24 hours
        _rolePermissionService = rolePermissionService;
        _userRepository = userRepository;
    }

    public async Task<string> GenerateToken(string email, string role)
    {
        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_secretKey));
        SigningCredentials? credentials = new(key, SecurityAlgorithms.HmacSha256);

        List<Claim>? claims = new()
        {
            new("email", email),
            new("role", role)   
        };

        User? user = await _userRepository.GetByStringAsync(u => u.Email == email);

        RolePermissionViewModel permissions =  await _rolePermissionService.Get(user.RoleId);

        // Add permissions to claims
        foreach (PermissionViewModel permission in permissions.Permissions)
        {
            if (permission.CanView)
            {
                claims.Add(new Claim("permission", $"View_{permission.PermissionName.Replace(" ", "_")}"));
            }
            if (permission.CanEdit)
            {
                claims.Add(new Claim("permission", $"Edit_{permission.PermissionName.Replace(" ", "_")}"));
            }
            if (permission.CanDelete)
            {
                claims.Add(new Claim("permission", $"Delete_{permission.PermissionName.Replace(" ", "_")}"));
            }
        }

        JwtSecurityToken? token = new(
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
        JwtSecurityTokenHandler? handler = new JwtSecurityTokenHandler();
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

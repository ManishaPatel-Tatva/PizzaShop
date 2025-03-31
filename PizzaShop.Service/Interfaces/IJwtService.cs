using System.Security.Claims;

namespace PizzaShop.Service.Interfaces;

public interface IJwtService
{
    Task<string> GenerateToken(string email, string role);

    ClaimsPrincipal? GetClaimsFromToken(string token);

    string? GetClaimValue(string token, string claimType);
}

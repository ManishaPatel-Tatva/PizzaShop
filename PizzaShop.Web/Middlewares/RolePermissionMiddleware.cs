using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using PizzaShop.Service.Interfaces;
using System.IdentityModel.Tokens.Jwt;

namespace PizzaShop.Web.Middlewares;

public class RolePermissionMiddleware
{
    private readonly RequestDelegate _next;

    public RolePermissionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IJwtService jwtService)
    {   
        string? token = context.Request.Cookies["authToken"];

        if (!string.IsNullOrEmpty(token))
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken = handler.ReadJwtToken(token);

            string? roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "role")?.Value;

            if (!string.IsNullOrEmpty(roleClaim))
            {
                context.Items["UserRole"] = roleClaim;
            }
        }

        await _next(context);
    }
}

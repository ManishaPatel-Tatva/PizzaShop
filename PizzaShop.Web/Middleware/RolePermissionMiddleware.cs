using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using PizzaShop.Service.Interfaces;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Service.Exceptions;
using PizzaShop.Service.Common;

namespace PizzaShop.Web.Middleware;

public class RolePermissionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMemoryCache _cache;

    public RolePermissionMiddleware(RequestDelegate next, IMemoryCache cache)
    {
        _next = next;
        _cache = cache;
    }

    public async Task InvokeAsync(HttpContext context, IRolePermissionService rolePermissionService)
    {
        string? token = context.Request.Cookies["authToken"];

        if (!string.IsNullOrEmpty(token))
        {
            JwtSecurityTokenHandler handler = new();
            JwtSecurityToken jwtToken = handler.ReadJwtToken(token);

            string? roleIdStr = jwtToken.Claims.FirstOrDefault(c => c.Type == "roleId")?.Value;
            string? roleName = jwtToken.Claims.FirstOrDefault(c => c.Type == "role")?.Value;

            if (!string.IsNullOrEmpty(roleIdStr) && long.TryParse(roleIdStr, out long roleId))
            {
                RolePermissionViewModel permissions = await rolePermissionService.Get(roleId)
                ?? throw new NotFoundException(NotificationMessages.NotFound.Replace("{0}","Permissions"));
                // Build all claims
                List<Claim>? claims = jwtToken.Claims.ToList();

                if (!string.IsNullOrEmpty(roleName))
                {
                    claims.Add(new Claim(ClaimTypes.Role, roleName));
                }

                foreach (PermissionViewModel? permission in permissions.Permissions)
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

                // Create identity with correct role claim type
                ClaimsIdentity identity = new(claims, "jwt", ClaimTypes.Name, ClaimTypes.Role);
                context.User = new ClaimsPrincipal(identity);
            }
        }
        await _next(context);
    }
}

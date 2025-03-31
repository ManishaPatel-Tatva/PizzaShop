using PizzaShop.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Security.Claims;

namespace PizzaShop.Web.Filters;

public class CustomAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    private readonly string _requiredPermission;

    public CustomAuthorizeAttribute(string requiredPermission)
    {
        _requiredPermission = requiredPermission;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        IJwtService? jwtService = context.HttpContext.RequestServices.GetService(typeof(IJwtService)) as IJwtService;
        HttpContext httpContext = context.HttpContext;

        string? token = httpContext.Request.Cookies["authToken"];
        if (string.IsNullOrEmpty(token))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        ClaimsPrincipal? claims = jwtService?.GetClaimsFromToken(token);
        if (claims == null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        List<string> permissions = claims.Claims.Where(c => c.Type == "permission").Select(c => c.Value).ToList();

        if (!permissions.Contains(_requiredPermission))
        {
            context.Result = new ForbidResult();
        }
    }
}

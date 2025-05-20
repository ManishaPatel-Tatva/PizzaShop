using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace PizzaShop.Web.Filters;

[AttributeUsage(AttributeTargets.All)]
public class CustomAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    private readonly string _requiredPermission;

    public CustomAuthorizeAttribute(string requiredPermission)
    {
        _requiredPermission = requiredPermission;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        ClaimsPrincipal? user = context.HttpContext.User;

        if (!user.Identity?.IsAuthenticated ?? false)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        bool hasPermission = user.Claims.Any(c => c.Type == "permission" && c.Value == _requiredPermission);

        if (!hasPermission)
        {
            context.Result = new ForbidResult();
        }
    }
}

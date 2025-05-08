using System.Net;
using System.Text.Json;
using PizzaShop.Service.Exceptions;

namespace PizzaShop.Web.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }
 
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        HttpStatusCode code;
        string message;
 
        switch (exception)
        {
            case NotFoundException:
                code = HttpStatusCode.NotFound;
                message = exception.Message;
                break;
            case BadRequestException:
                code = HttpStatusCode.BadRequest;
                message = exception.Message;
                break;
            case UnauthorizedException:
                code = HttpStatusCode.Unauthorized;
                message = exception.Message;
                break;
            default:
                code = HttpStatusCode.InternalServerError;
                message = "An unexpected error occurred. Please try again later.";
                break;
        }
 
        _logger.LogError(exception, "An unhandled exception occurred.");
 
        bool isAjax = context.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
 
        if (isAjax)
        {
            // For AJAX - return JSON response
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 200; // Always OK (to avoid redirect issues)
 
            context.Response.Headers.Add("X-Error", "true");
 
            var jsonResponse = new
            {
                success = false,
                statusCode = (int)code,
                error = message
            };
            string json = JsonSerializer.Serialize(jsonResponse);
            await context.Response.WriteAsync(json);
        }
        else
        {
            // For Normal Requests - use TempData for Toastr
            context.Response.Redirect($"/Auth/HandleErrorWithToast?message={Uri.EscapeDataString(message)}");
        }
    }
 
 
}
 

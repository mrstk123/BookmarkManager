using System.Net;
using System.Text.Json;
using BookmarkManager.Application.Exceptions;

namespace BookmarkManager.WebAPI.Middleware;

/// <summary>
/// Middleware that catches unhandled exceptions and returns consistent error responses.
/// Maps specific exception types to appropriate HTTP status codes.
/// </summary>
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
        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new ErrorResponse
        {
            StatusCode = (int)HttpStatusCode.InternalServerError,
            Message = "An unexpected error occurred. Please try again later.",
            Details = null  // never expose internal details in production
        };

        switch (exception)
        {
            // Intentional business-rule violations (BusinessRuleException) → 400
            case BusinessRuleException:
                _logger.LogWarning("Domain rule violation: {Message}", exception.Message);
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = exception.Message;
                break;

            case UnauthorizedAccessException:
                _logger.LogWarning("Unauthorized access attempt.");
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse.Message = "Unauthorized access.";
                break;

            case KeyNotFoundException:
                _logger.LogWarning("Resource not found: {Message}", exception.Message);
                response.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse.Message = exception.Message;
                break;

            case FluentValidation.ValidationException validationEx:
                _logger.LogWarning("Validation failed: {Message}", exception.Message);
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = "Validation failed.";
                errorResponse.Errors = validationEx.Errors
                    .Select(e => e.ErrorMessage)
                    .ToList();
                break;

            // Anything else (including InvalidOperationException from the runtime) stays 500
            default:
                _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
        }

        var json = JsonSerializer.Serialize(errorResponse);
        await response.WriteAsync(json);
    }
}

/// <summary>
/// Extension method for registering the exception handling middleware.
/// </summary>
public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
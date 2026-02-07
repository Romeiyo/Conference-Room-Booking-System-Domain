using System.Net;
using System.Text.Json;
using ConferenceRoomBookingSystem;
using Microsoft.Extensions.Logging;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        // response.StatusCode = exception switch
        // {
        //     InvalidBookingException => StatusCodes.Status422UnprocessableEntity,
            
        //     _ => StatusCodes.Status500InternalServerError,

            
        // };

        response.StatusCode = exception switch
        {
            InvalidBookingException => StatusCodes.Status422UnprocessableEntity,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            BookingConflictException => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError // Default case for all other exceptions
        };

        var payload = new 
        {
            error = exception.GetType().Name,
            detail = GetErrorMessage(exception),
            category = GetErrorCategory(exception),
            traceId = context.TraceIdentifier
        };

        return response.WriteAsync(JsonSerializer.Serialize(payload));
    }

    private static ErrorCategory GetErrorCategory(Exception ex)
    {
        return ex switch
        {
            ArgumentException => ErrorCategory.ValidationError,
            InvalidBookingException => ErrorCategory.ValidationError,
            BookingConflictException => ErrorCategory.BusinessRuleViolation,
            UnauthorizedAccessException => ErrorCategory.BusinessRuleViolation,
            IOException => ErrorCategory.InfrastructureFailure, 
            JsonException => ErrorCategory.InfrastructureFailure, 
            _ => ErrorCategory.UnexpectedError
        };
    }
    
    // Helper method for smart messages:
    private static string GetErrorMessage(Exception ex)
    {
        var category = GetErrorCategory(ex);
        
        return category switch
        {
            ErrorCategory.ValidationError => ex.Message, // Show detailed message
            ErrorCategory.BusinessRuleViolation => ex.Message, // Show rule violation
            ErrorCategory.InfrastructureFailure => "A system error occurred. Please try again later.",
            ErrorCategory.UnexpectedError => "An unexpected error occurred.",
            _ => "An error occurred."
        };
    }

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
            _logger.LogError(ex, 
            "Error processing request {Method} {Path}. Error category: {Category}",
            context.Request.Method,
            context.Request.Path,
            GetErrorCategory(ex));
            
            await HandleExceptionAsync(context, ex);
        }
    }
}
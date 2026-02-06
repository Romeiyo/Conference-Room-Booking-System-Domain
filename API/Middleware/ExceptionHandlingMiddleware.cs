using System.Net;
using System.Text.Json;
using ConferenceRoomBookingSystem;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

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
            detail = exception.Message
        };

        return response.WriteAsync(JsonSerializer.Serialize(payload));
    }

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
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
}
using System.Text.Json;
using TaskTracker.Api.Errors;
using TaskTracker.Api.Exceptions;

namespace TaskTracker.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch(Exception ex)
        {
            var traceId = context.TraceIdentifier;
            var(statusCode, message, errors) = MapException(ex);

            if(statusCode >= 500)
                _logger.LogError(ex, "Unhandled exception. TraceId={TraceId}", traceId);

            else
                _logger.LogWarning(ex, "Handled exception. TraceId={TraceId}", traceId);

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var payload = new ApiErrorResponse(
                TraceId: traceId,
                Message: message,
                Errors: errors
            );

            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }
    }

    public (int statusCode, string message, Dictionary<string, string[]>? errors)
        MapException(Exception ex)
    {
        if (ex is OperationCanceledException or TaskCanceledException)
            return (499, "Request was cancelled by client", null);

        if (ex is ValidationException ve)
            return (StatusCodes.Status400BadRequest, ve.Message, null); 

        if(ex is NotFoundException nf)        
            return (StatusCodes.Status404NotFound, nf.Message, null);
        

        return (StatusCodes.Status500InternalServerError, "Unexpected error", null);
    }
}
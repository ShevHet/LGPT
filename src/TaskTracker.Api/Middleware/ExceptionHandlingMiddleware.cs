using System.Text.Json;
using TaskTracker.Application.Exceptions;
using TaskTracker.Api.Errors;

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

            if(statusCode >= StatusCodes.Status500InternalServerError)
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

            await context.Response.WriteAsJsonAsync(payload);
        }
    }

    public static (int statusCode, string message, Dictionary<string, string[]>? errors)
        MapException(Exception ex)
    {
        return ex switch
        {
            ValidationException validationException =>
                (StatusCodes.Status400BadRequest, validationException.Message, null),
            NotFoundException notFoundException =>
                (StatusCodes.Status404NotFound, notFoundException.Message, null),
            ConflictException conflictException =>
                (StatusCodes.Status409Conflict, conflictException.Message, null),
            BadHttpRequestException =>
                (StatusCodes.Status400BadRequest, "Invalid HTTP request", null),
            System.Text.Json.JsonException =>
                (StatusCodes.Status400BadRequest, "Invalid JSON paylad", null),
            OperationCanceledException or TaskCanceledException =>
                (StatusCodes.Status500InternalServerError, "Unexpected error", null)
        };
    }
}

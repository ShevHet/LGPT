using System.Text.Json;
using TaskTracker.Api.Errors;

namespace TaskTracker.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequesDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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

            _logger.LogError(ex, "Unhandled exception. TraceId={TraceId}", traceId);
            
            var(statusCode, message, errors) = MapException(ex);
        }
    }

    public (int statusCode, string message, Dictionary<string, string[]>? errors)
        MapException(Exception ex)
    {
        if(ex is ArgumentException arg)
        {
            var field = arg.ParamName ?? "request"
            return (
                StatusCodes.StatusCode400BadRequest,
                "Validation Errors",
                new Dictionary<string, string[]>
                {
                    [field] = new[] { arg.Message }
                }
            );
        }

        return (
            StatusCodes.Status500InternalServerError,
            "Unexpected error",
            null
        );
    }
}
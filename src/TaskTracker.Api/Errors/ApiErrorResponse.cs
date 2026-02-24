namespace TaskTracker.Api.Errors;

public sealed record ApiErrorsResponse(
    string TraceId,
    string Message,
    Dicionary<string, string[]>? Errors = null
);
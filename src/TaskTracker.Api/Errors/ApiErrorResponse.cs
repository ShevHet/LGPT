namespace TaskTracker.Api.Errors;

public record ApiErrorResponse(string TraceId, string Message, Dictionary<string, string[]>? Errors);
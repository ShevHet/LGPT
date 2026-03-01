namespace TaskTracker.Api.Errors
{
    public sealed record ApiErrorResponse(
        string TraceId,
        string Message,
        Dictionary<string, string[]>? Errors = null
    );
}
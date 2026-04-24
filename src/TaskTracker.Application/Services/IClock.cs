namespace TaskTracker.Application.Services;

public interface IClock
{
    DateTime UtcNow { get; }
}

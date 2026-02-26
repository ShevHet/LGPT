public sealed class TaskTrackerOptions
{
    public const string SectionName = "TaskTracker";

    public int MaxTasksLimit { get; init; } = 100;

    public string DefaultTitlePrefix { get; init; } = "";
}
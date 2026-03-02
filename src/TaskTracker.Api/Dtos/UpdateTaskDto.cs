namespace TaskTracker.Api.Dtos;

public class UpdateTaskDto
{
    public string Title { get; set; } = "";

    public bool IsDone { get; set; }
}

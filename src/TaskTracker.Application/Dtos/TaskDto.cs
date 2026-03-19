namespace TaskTracker.Application.Dtos;

public class TaskDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public bool IsDone { get; set; }
}

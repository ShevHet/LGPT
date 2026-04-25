namespace TaskTracker.Domain.Models;

public class TaskItem
{
    public int Id {  get; set; }

    public int ProjectId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public TaskStatus Status { get; set; } = TaskStatus.New;

    public DateTime CreatedAt { get; set; }
   
    public DateTime UpdatedAt { get; set; }

    public Project Project { get; set; } = null!;

}

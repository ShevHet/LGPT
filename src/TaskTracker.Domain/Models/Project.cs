namespace TaskTracker.Domain.Models;

public class Project
{
    public int Id {  get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTime CreatedAt{ get; set; }

    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
using DomainTaskStatus = TaskTracker.Domain.Models.TaskStatus;

namespace TaskTracker.Application.Dtos
{
    public class UpdateTaskRequestDto
    {
        public int ProjectId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DomainTaskStatus Status { get; set; }
    }
}

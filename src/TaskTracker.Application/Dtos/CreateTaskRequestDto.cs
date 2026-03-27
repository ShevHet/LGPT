using System.ComponentModel.DataAnnotations;
using DomainTaskStatus = TaskTracker.Domain.Models.TaskStatus;

namespace TaskTracker.Application.Dtos
{
    public class CreateTaskRequestDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "ProjectId must be > 0")]
        public int ProjectId { get; set; }
        
        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string Title { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Description { get; set; }

        [EnumDataType(typeof(DomainTaskStatus))]
        public DomainTaskStatus Status { get; set; }
    }
}

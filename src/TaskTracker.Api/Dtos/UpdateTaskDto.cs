using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Api.Dtos;

public class UpdateTaskDto
{
    [Required]
    [MinLength(3)]
    public string Title { get; set; } = "";

    public bool IsDone { get; set; }
}
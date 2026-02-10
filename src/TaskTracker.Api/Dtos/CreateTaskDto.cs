using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Api.Dtos;

public class CreateTaskDto
{
    [Required]
    [MinLength(3)]
    public string Title { get; set; } = "";
}
using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Api.Dtos;

public class CreateTaskDto
{
    public string Title { get; set; } = "";
}
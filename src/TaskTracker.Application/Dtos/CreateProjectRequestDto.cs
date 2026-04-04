using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Application.Dtos
{
    /// <summary>
    /// Data to create project.
    /// </summary>
    public class CreateProjectRequestDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
    }
}

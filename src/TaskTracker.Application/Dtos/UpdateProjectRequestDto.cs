using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Application.Dtos
{
    /// <summary>
    /// Data to update project.
    /// </summary>
    public class UpdateProjectRequestDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
    }
}

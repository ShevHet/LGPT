using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Application.Dtos
{
    public class UpdateProjectRequestDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
    }
}

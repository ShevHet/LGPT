using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Application.Dtos
{
    public class CreateProjectRequestDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
    }
}

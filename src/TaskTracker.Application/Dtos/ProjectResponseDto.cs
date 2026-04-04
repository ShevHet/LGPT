namespace TaskTracker.Application.Dtos
{
    /// <summary>
    /// Project data.
    /// </summary>
    public class ProjectResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}

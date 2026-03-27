using TaskTracker.Application.Dtos;

namespace TaskTracker.Application.Services
{
    public interface IProjectService
    {
        Task<IReadOnlyCollection<ProjectResponseDto>> GetAllAsync(CancellationToken cancellationToken);
        Task<ProjectResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<ProjectResponseDto> CreateAsync(CreateProjectRequestDto request, CancellationToken cancellationToken);
        Task<bool> UpdateAsync(int id, UpdateProjectRequestDto request, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
        Task<IReadOnlyCollection<TaskResponseDto>> GetTasksByProjectIdAsync(int projectId, CancellationToken cancellationToken);
    }
}
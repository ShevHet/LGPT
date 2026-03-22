using TaskTracker.Application.Dtos;

namespace TaskTracker.Application.Services;

public interface ITaskService
{
    Task<IReadOnlyCollection<TaskResponseDto>> GetAllAsync(CancellationToken ct);
    Task<TaskResponseDto> GetByIdAsync(int id, CancellationToken ct);
    Task<TaskResponseDto> CreateAsync(CreateTaskRequestDto request, CancellationToken ct);
    Task<bool> UpdateAsync(int id, UpdateTaskRequestDto request, CancellationToken ct);
    Task<bool> DeleteAsync(int id, CancellationToken ct);
}
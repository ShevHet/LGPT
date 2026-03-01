using TaskTracker.Api.Dtos;

namespace TaskTracker.Api.Services;

public interface ITaskService
{
	Task<List<TaskDto>> GetAllAsync(CancellationToken ct);
	Task<TaskDto?> GetByIdAsync(int id, CancellationToken ct);
	Task<TaskDto> CreateAsync(string title, CancellationToken ct);
	Task<bool> UpdateAsync(int id, string title, bool isDone, CancellationToken ct);
	Task<bool> DeleteAsync(int id, CancellationToken ct);
}
using TaskTracker.Api.Dtos;

namespace TaskTracker.Api.Services;

public interface ITaskService
{
	List<TaskDto> GetAll();
	TaskDto? GetById(int id);
	TaskDto Create(string title);
	bool Update(int id, string title, bool isDone);
	bool Delete(int id);
}
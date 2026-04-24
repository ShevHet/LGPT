using Moq;
using TaskTracker.Application.Exceptions;
using TaskTracker.Application.Services;
using TaskTracker.Domain.Models;
using DomainTaskStatus = TaskTracker.Domain.Models.TaskStatus;

namespace TaskTracker.Tests.Services.Tasks
{
    public class TaskServiceDeleteTests
    {
        [Fact]
        public async Task DeleteAsync_ShouldCallRemove_WhenTaskExists()
        {
            var existingTask = BuildExistingTask();
            var taskRepoMock = CreateRepositoryMockForSuccessfulDelete(existingTask);
            var projectRepoMock = new Mock<IProjectRepository>(MockBehavior.Strict);
            var clockMock = new Mock<IClock>(MockBehavior.Strict);
            var service = new TaskService(taskRepoMock.Object, projectRepoMock.Object, clockMock.Object);

            var result = await service.DeleteAsync(existingTask.Id, CancellationToken.None);

            Assert.True(result);
            taskRepoMock.Verify(repository => repository.Remove(existingTask), Times.Once);
            projectRepoMock.VerifyNoOtherCalls();
            clockMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task DeleteAsync_ShouldCallSaveChangesOnce_WhenTaskExists()
        {
            var existingTask = BuildExistingTask();
            var taskRepoMock = CreateRepositoryMockForSuccessfulDelete(existingTask);
            var projectRepoMock = new Mock<IProjectRepository>(MockBehavior.Strict);
            var clockMock = new Mock<IClock>(MockBehavior.Strict);
            var service = new TaskService(taskRepoMock.Object, projectRepoMock.Object, clockMock.Object);

            await service.DeleteAsync(existingTask.Id, CancellationToken.None);

            taskRepoMock.Verify(repository => repository.GetByIdAsync(existingTask.Id, It.IsAny<CancellationToken>()), Times.Once);
            taskRepoMock.Verify(repository => repository.Remove(existingTask), Times.Once);
            taskRepoMock.Verify(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            taskRepoMock.VerifyNoOtherCalls();
            projectRepoMock.VerifyNoOtherCalls();
            clockMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowNotFoundException_WhenTaskDoesNotExist()
        {
            const int missingTaskId = 77;
            var taskRepoMock = new Mock<ITaskRepository>(MockBehavior.Strict);
            var projectRepoMock = new Mock<IProjectRepository>(MockBehavior.Strict);
            var clockMock = new Mock<IClock>(MockBehavior.Strict);

            taskRepoMock
                .Setup(repository => repository.GetByIdAsync(missingTaskId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((TaskItem?)null);

            var service = new TaskService(taskRepoMock.Object, projectRepoMock.Object, clockMock.Object);

            var exception = await Assert.ThrowsAsync<NotFoundException>(
                () => service.DeleteAsync(missingTaskId, CancellationToken.None));

            Assert.Equal("Task with id = 77 was not found", exception.Message);
            taskRepoMock.Verify(repository => repository.GetByIdAsync(missingTaskId, It.IsAny<CancellationToken>()), Times.Once);
            taskRepoMock.VerifyNoOtherCalls();
            projectRepoMock.VerifyNoOtherCalls();
            clockMock.VerifyNoOtherCalls();
        }

        private static TaskItem BuildExistingTask() => new()
        {
            Id = 21,
            ProjectId = 3,
            Title = "Remove",
            Description = "Delete flow happy-path",
            Status = DomainTaskStatus.InProgress,
            CreatedAt = new DateTime(2026, 04, 10, 8, 30, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2026, 04, 12, 9, 0, 0, DateTimeKind.Utc),
            Project = new Project
            {
                Id = 3,
                Name = "Data Platform"
            }
        };

        private static Mock<ITaskRepository> CreateRepositoryMockForSuccessfulDelete(TaskItem task)
        {
            var taskRepoMock = new Mock<ITaskRepository>(MockBehavior.Strict);

            taskRepoMock
                .Setup(repository => repository.GetByIdAsync(task.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(task);

            taskRepoMock
                .Setup(repository => repository.Remove(task));

            taskRepoMock
                .Setup(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            return taskRepoMock;
        }
    }
}

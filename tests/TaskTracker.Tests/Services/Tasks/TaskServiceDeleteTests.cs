using Moq;
using TaskTracker.Application.Exceptions;
using TaskTracker.Application.Services;
using TaskTracker.Domain.Models;
using DomainTaskStatus = TaskTracker.Domain.Models.TaskStatus;
using static TaskTracker.Tests.Services.Tasks.TaskServiceTestHelpers;

namespace TaskTracker.Tests.Services.Tasks
{
    public class TaskServiceDeleteTests
    {
        [Fact]
        public async Task DeleteAsync_ShouldCallRemove_WhenTaskExists()
        {
            var existingTask = BuildExistingTask();
            var taskRepoMock = CreateRepositoryMockForSuccessfulDelete(existingTask);
            var projectRepoMock = CreateStrictProjectRepositoryMock();
            var clockMock = CreateStrictClockMock();
            var service = CreateService(taskRepoMock, projectRepoMock, clockMock);

            var result = await service.DeleteAsync(existingTask.Id, CancellationToken.None);

            Assert.True(result);
            taskRepoMock.Verify(repository => repository.GetByIdAsync(existingTask.Id, It.IsAny<CancellationToken>()), Times.Once);
            taskRepoMock.Verify(repository => repository.Remove(existingTask), Times.Once);
            taskRepoMock.Verify(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            VerifyNoDependencyCalls(taskRepoMock, projectRepoMock, clockMock);
        }

        [Fact]
        public async Task DeleteAsync_ShouldCallSaveChangesOnce_WhenTaskExists()
        {
            var existingTask = BuildExistingTask();
            var taskRepoMock = CreateRepositoryMockForSuccessfulDelete(existingTask);
            var projectRepoMock = CreateStrictProjectRepositoryMock();
            var clockMock = CreateStrictClockMock();
            var service = CreateService(taskRepoMock, projectRepoMock, clockMock);

            await service.DeleteAsync(existingTask.Id, CancellationToken.None);

            taskRepoMock.Verify(repository => repository.GetByIdAsync(existingTask.Id, It.IsAny<CancellationToken>()), Times.Once);
            taskRepoMock.Verify(repository => repository.Remove(existingTask), Times.Once);
            taskRepoMock.Verify(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            VerifyNoDependencyCalls(taskRepoMock, projectRepoMock, clockMock);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowNotFoundException_WhenTaskDoesNotExist()
        {
            const int missingTaskId = 77;
            var taskRepoMock = CreateStrictTaskRepositoryMock();
            var projectRepoMock = CreateStrictProjectRepositoryMock();
            var clockMock = CreateStrictClockMock();

            taskRepoMock
                .Setup(repository => repository.GetByIdAsync(missingTaskId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((TaskItem?)null);

            var service = CreateService(taskRepoMock, projectRepoMock, clockMock);

            var exception = await Assert.ThrowsAsync<NotFoundException>(
                () => service.DeleteAsync(missingTaskId, CancellationToken.None));

            Assert.Equal("Task with id = 77 was not found", exception.Message);
            taskRepoMock.Verify(repository => repository.GetByIdAsync(missingTaskId, It.IsAny<CancellationToken>()), Times.Once);
            VerifyNoDependencyCalls(taskRepoMock, projectRepoMock, clockMock);
        }

        private static Mock<ITaskRepository> CreateRepositoryMockForSuccessfulDelete(TaskItem task)
        {
            var taskRepoMock = CreateStrictTaskRepositoryMock();

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

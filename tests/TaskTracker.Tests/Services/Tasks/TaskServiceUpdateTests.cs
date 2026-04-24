using Moq;
using TaskTracker.Application.Dtos;
using TaskTracker.Application.Exceptions;
using TaskTracker.Application.Services;
using TaskTracker.Domain.Models;
using DomainTaskStatus = TaskTracker.Domain.Models.TaskStatus;
using static TaskTracker.Tests.Services.Tasks.TaskServiceTestHelpers;

namespace TaskTracker.Tests.Services.Tasks
{
    public class TaskServiceUpdateTests
    {
        [Fact]
        public async Task UpdateAsync_ShouldUpdateTaskFields_WhenTaskExists()
        {
            var existingTask = BuildExistingTask();
            var originalCreatedAt = existingTask.CreatedAt;
            var request = BuildUpdateRequest();
            var taskRepoMock = CreateTaskRepositoryMockForSuccessfulUpdate(existingTask);
            var projectRepoMock = CreateProjectRepositoryMockWithExistsResult(request.ProjectId, true);
            var clockMock = CreateClockMock(FixedUtcNow);
            var service = CreateService(taskRepoMock, projectRepoMock, clockMock);

            var result = await service.UpdateAsync(existingTask.Id, request, CancellationToken.None);

            Assert.True(result);
            Assert.Equal(request.ProjectId, existingTask.ProjectId);
            Assert.Equal(request.Title, existingTask.Title);
            Assert.Equal(request.Description, existingTask.Description);
            Assert.Equal(request.Status, existingTask.Status);
            Assert.Equal(originalCreatedAt, existingTask.CreatedAt);
            Assert.Equal(FixedUtcNow, existingTask.UpdatedAt);
        }

        [Fact]
        public async Task UpdateAsync_ShouldCallSaveChangesOnce_WhenTaskExists()
        {
            var existingTask = BuildExistingTask();
            var fixedUtcNow = new DateTime(2026, 04, 21, 15, 30, 0, DateTimeKind.Utc);
            var request = BuildUpdateRequest();
            var taskRepoMock = CreateTaskRepositoryMockForSuccessfulUpdate(existingTask);
            var projectRepoMock = CreateProjectRepositoryMockWithExistsResult(request.ProjectId, true);
            var clockMock = CreateClockMock(fixedUtcNow);
            var service = CreateService(taskRepoMock, projectRepoMock, clockMock);

            await service.UpdateAsync(existingTask.Id, request, CancellationToken.None);

            taskRepoMock.Verify(repository => repository.GetByIdAsync(existingTask.Id, It.IsAny<CancellationToken>()), Times.Once);
            projectRepoMock.Verify(repository => repository.ExistsAsync(request.ProjectId, It.IsAny<CancellationToken>()), Times.Once);
            clockMock.VerifyGet(clock => clock.UtcNow, Times.Once);
            taskRepoMock.Verify(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            taskRepoMock.VerifyNoOtherCalls();
            projectRepoMock.VerifyNoOtherCalls();
            clockMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowNotFoundException_WhenTaskDoesNotExist()
        {
            const int missingTaskId = 99;
            var request = BuildUpdateRequest();
            var taskRepoMock = CreateStrictTaskRepositoryMock();
            var projectRepoMock = CreateStrictProjectRepositoryMock();
            var clockMock = CreateStrictClockMock();

            taskRepoMock
                .Setup(repository => repository.GetByIdAsync(missingTaskId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((TaskItem?)null);

            var service = CreateService(taskRepoMock, projectRepoMock, clockMock);

            var exception = await Assert.ThrowsAsync<NotFoundException>(
                () => service.UpdateAsync(missingTaskId, request, CancellationToken.None));

            Assert.Equal("Task with id = 99 was not found", exception.Message);
            taskRepoMock.Verify(repository => repository.GetByIdAsync(missingTaskId, It.IsAny<CancellationToken>()), Times.Once);
            VerifyNoDependencyCalls(taskRepoMock, projectRepoMock, clockMock); 
        }

        private static Mock<ITaskRepository> CreateTaskRepositoryMockForSuccessfulUpdate(TaskItem task)
        {
            var taskRepoMock = new Mock<ITaskRepository>(MockBehavior.Strict);

            taskRepoMock
                .Setup(repository => repository.GetByIdAsync(task.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(task);

            taskRepoMock
                .Setup(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            return taskRepoMock;
        }
    }
}

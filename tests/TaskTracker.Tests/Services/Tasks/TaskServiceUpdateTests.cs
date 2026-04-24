using Moq;
using TaskTracker.Application.Dtos;
using TaskTracker.Application.Exceptions;
using TaskTracker.Application.Services;
using TaskTracker.Domain.Models;
using DomainTaskStatus = TaskTracker.Domain.Models.TaskStatus;

namespace TaskTracker.Tests.Services.Tasks
{
    public class TaskServiceUpdateTests
    {
        [Fact]
        public async Task UpdateAsync_ShouldUpdateTaskFields_WhenTaskExists()
        {
            var existingTask = BuildExistingTask();
            var originalCreatedAt = existingTask.CreatedAt;
            var fixedUtcNow = new DateTime(2026, 04, 21, 15, 30, 0, DateTimeKind.Utc);
            var request = BuildRequest();
            var taskRepoMock = CreateTaskRepositoryMockForSuccessfulUpdate(existingTask);
            var projectRepoMock = CreateProjectRepositoryMockForExistingProject(request.ProjectId);
            var clockMock = CreateClockMock(fixedUtcNow);
            var service = new TaskService(taskRepoMock.Object, projectRepoMock.Object, clockMock.Object);

            var result = await service.UpdateAsync(existingTask.Id, request, CancellationToken.None);

            Assert.True(result);
            Assert.Equal(request.ProjectId, existingTask.ProjectId);
            Assert.Equal(request.Title, existingTask.Title);
            Assert.Equal(request.Description, existingTask.Description);
            Assert.Equal(request.Status, existingTask.Status);
            Assert.Equal(originalCreatedAt, existingTask.CreatedAt);
            Assert.Equal(fixedUtcNow, existingTask.UpdatedAt);
        }

        [Fact]
        public async Task UpdateAsync_ShouldCallSaveChangesOnce_WhenTaskExists()
        {
            var existingTask = BuildExistingTask();
            var fixedUtcNow = new DateTime(2026, 04, 21, 15, 30, 0, DateTimeKind.Utc);
            var request = BuildRequest();
            var taskRepoMock = CreateTaskRepositoryMockForSuccessfulUpdate(existingTask);
            var projectRepoMock = CreateProjectRepositoryMockForExistingProject(request.ProjectId);
            var clockMock = CreateClockMock(fixedUtcNow);
            var service = new TaskService(taskRepoMock.Object, projectRepoMock.Object, clockMock.Object);

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
            var request = BuildRequest();
            var taskRepoMock = new Mock<ITaskRepository>(MockBehavior.Strict);
            var projectRepoMock = new Mock<IProjectRepository>(MockBehavior.Strict);
            var clockMock = new Mock<IClock>(MockBehavior.Strict);

            taskRepoMock
                .Setup(repository => repository.GetByIdAsync(missingTaskId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((TaskItem?)null);

            var service = new TaskService(taskRepoMock.Object, projectRepoMock.Object, clockMock.Object);

            var exception = await Assert.ThrowsAsync<NotFoundException>(
                () => service.UpdateAsync(missingTaskId, request, CancellationToken.None));

            Assert.Equal("Task with id = 99 was not found", exception.Message);
            taskRepoMock.Verify(repository => repository.GetByIdAsync(missingTaskId, It.IsAny<CancellationToken>()), Times.Once);
            taskRepoMock.VerifyNoOtherCalls();
            projectRepoMock.VerifyNoOtherCalls();
            clockMock.VerifyNoOtherCalls();
        }

        private static UpdateTaskRequestDto BuildRequest() => new()
        {
            ProjectId = 12,
            Title = "Ship update flow",
            Description = "Refresh task fields from dto",
            Status = DomainTaskStatus.Done
        };

        private static TaskItem BuildExistingTask() => new()
        {
            Id = 42,
            ProjectId = 7,
            Title = "Draft API contract",
            Description = "Initial version",
            Status = DomainTaskStatus.New,
            CreatedAt = new DateTime(2026, 04, 10, 8, 30, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2026, 04, 11, 9, 0, 0, DateTimeKind.Utc),
            Project = new Project
            {
                Id = 7,
                Name = "Website Redesign"
            }
        };

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

        private static Mock<IProjectRepository> CreateProjectRepositoryMockForExistingProject(int projectId)
        {
            var projectRepoMock = new Mock<IProjectRepository>(MockBehavior.Strict);

            projectRepoMock
                .Setup(repository => repository.ExistsAsync(projectId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            return projectRepoMock;
        }

        private static Mock<IClock> CreateClockMock(DateTime utcNow)
        {
            var clockMock = new Mock<IClock>(MockBehavior.Strict);

            clockMock
                .SetupGet(clock => clock.UtcNow)
                .Returns(utcNow);

            return clockMock;
        }
    }
}

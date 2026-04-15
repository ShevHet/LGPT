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
            //Arrange
            var existingTask = BuildExistingTask();
            var originalCreatedAt = existingTask.CreatedAt;
            var originalUpdateAt = existingTask.UpdatedAt;
            var request = BuildRequest();
            var repoMock = CreateRepositoryMockForSuccessfulUpdate(existingTask, request.ProjectId);
            var service = new TaskService(repoMock.Object);

            //Act
            var result = await service.UpdateAsync(existingTask.Id,request,CancellationToken.None);

            //Assert
            Assert.True(result);
            Assert.Equal(request.ProjectId, existingTask.ProjectId);
            Assert.Equal(request.Title, existingTask.Title);
            Assert.Equal(request.Description, existingTask.Description);
            Assert.Equal(request.Status, existingTask.Status);
            Assert.Equal(originalCreatedAt, existingTask.CreatedAt);
            Assert.True(originalUpdateAt < existingTask.UpdatedAt);
        }

        [Fact]
        public async Task UpdateAsync_ShouldCallSaveChangesOnce_WhenTaskExists()
        {
            //Arrange 
            var existingTask = BuildExistingTask();
            var request = BuildRequest();
            var repoMock = CreateRepositoryMockForSuccessfulUpdate(existingTask, request.ProjectId);
            var service = new TaskService(repoMock.Object);

            //Act
            await service.UpdateAsync(existingTask.Id, request,CancellationToken.None);

            //Assert
            repoMock.Verify(r => r.GetByIdAsync(existingTask.Id, It.IsAny<CancellationToken>()), Times.Once());
            repoMock.Verify(r => r.ProjectExistsAsync(request.ProjectId, It.IsAny<CancellationToken>()), Times.Once());
            repoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
            repoMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowNotFoundException_WhenTaskDoesNotExist()
        {
            //Arrange
            const int missingTaskId = 99;
            var request = BuildRequest();
            var repoMock = new Mock<ITaskRepository>(MockBehavior.Strict);

            repoMock
                .Setup(r => r.GetByIdAsync(missingTaskId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((TaskItem?)null);

            var service = new TaskService(repoMock.Object);

            //Act
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                ()=> service.UpdateAsync(missingTaskId, request, CancellationToken.None));

            //Assert
            Assert.Equal("Task with id = 99 was not found", exception.Message);
            repoMock.Verify(r => r.GetByIdAsync(missingTaskId, It.IsAny<CancellationToken>()), Times.Once);
            repoMock.VerifyNoOtherCalls();
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
            Title = "Draft API contract",
            Description = "Initial version",
            Status = DomainTaskStatus.New,
            CreatedAt = new DateTime(2026, 04, 10, 8, 30 ,0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2026, 04, 11, 9, 0 ,0, DateTimeKind.Utc),
            Project = new Project
            {
                Id = 7,
                Name = "Website Redesign"
            }   
        };

        private static Mock<ITaskRepository> CreateRepositoryMockForSuccessfulUpdate(
            TaskItem task,
            int newProjectId)
        {
            var repoMock = new Mock<ITaskRepository>(MockBehavior.Strict);

            repoMock
                .Setup(r => r.GetByIdAsync(task.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(task);

            repoMock
                .Setup(r => r.ProjectExistsAsync(newProjectId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            repoMock
                .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            return repoMock;
        }
    }
}

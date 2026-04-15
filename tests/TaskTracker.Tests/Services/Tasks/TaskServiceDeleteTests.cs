using Moq;
using Newtonsoft.Json.Serialization;
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
            //Arrange
            var existingTask = BuildExistingTask();
            var repoMock = CreateRepositoryMockForSuccessfulDelete(existingTask);
            var service = new TaskService(repoMock.Object);

            //Act
            var result = await service.DeleteAsync(existingTask.Id, CancellationToken.None);

            //Assert
            Assert.True(result);
            repoMock.Verify(r => r.Remove(existingTask), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldCallSaveChangesOnce_WhenTaskExists()
        {
            //Arrange
            var existingTask = BuildExistingTask();
            var repoMock = CreateRepositoryMockForSuccessfulDelete(existingTask);
            var service = new TaskService(repoMock.Object);

            //Act
            await service.DeleteAsync(existingTask.Id, CancellationToken.None);

            //Assert
            repoMock.Verify(r=>r.GetByIdAsync(existingTask.Id, It.IsAny<CancellationToken>()), Times.Once);
            repoMock.Verify(r=>r.Remove(existingTask), Times.Once);
            repoMock.Verify(r=>r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            repoMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowNotFoundException_WhenTaskDoesNotExist()
        {
            //Arrange
            const int missingTaskId = 77;
            var repoMock = new Mock<ITaskRepository>(MockBehavior.Strict);

            repoMock
                .Setup(r => r.GetByIdAsync(missingTaskId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((TaskItem?)null);

            var service = new TaskService(repoMock.Object);

            //Act
            var exception =  await Assert.ThrowsAsync<NotFoundException>( 
                () => service.DeleteAsync(missingTaskId, CancellationToken.None));

            //Assert
            Assert.Equal("Task with id = 77 was not found", exception.Message);
            repoMock.Verify(r=>r.GetByIdAsync(missingTaskId, It.IsAny<CancellationToken>()), Times.Once);
            repoMock.VerifyNoOtherCalls();
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
            var repoMock = new Mock<ITaskRepository>(MockBehavior.Strict);

            repoMock
                .Setup(r=>r.GetByIdAsync(task.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(task);

            repoMock
                .Setup(r => r.Remove(task));

            repoMock
                .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            return repoMock;
        }
    }
}

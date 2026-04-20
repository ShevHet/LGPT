using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Moq;
using TaskTracker.Application.Dtos;
using TaskTracker.Application.Services;
using TaskTracker.Domain.Models;
using DomainTaskStatus = TaskTracker.Domain.Models.TaskStatus;

namespace TaskTracker.Tests.Services.Tasks
{
    public class TaskServiceCreateValidationTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task CreateAsync_ShouldThrowValidationException_WhenProjectIdIsNotPositive(int projectId)
        {
            var request = BuildRequest();
            request.ProjectId = projectId;
            var repoMock = new Mock<ITaskRepository>(MockBehavior.Strict);
            var service = new TaskService(repoMock.Object);

            var exception = await Assert.ThrowsAsync<ValidationException>(
                () => service.CreateAsync(request, CancellationToken.None));

            Assert.Equal("ProjectId must bea positive number.", exception.Message);
            VerifyNoRepositoryCalls(repoMock);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task CreateAsync_ShouldThrowValidationException_WhenTitleIsNullEmptyOrWhiteSpace(string? title)
        {
            var request = BuildRequest();
            request.Title = title;
            var repoMock = new Mock<ITaskRepository>(MockBehavior.Strict);
            var servie = new TaskService(repoMock.Object);

            var exception = await Assert.ThrowsAsync<ValidationException>(
                () => servie.CreateAsync(request, CancellationToken.None));

            Assert.Equal("Title is required", exception.Message);
            VerifyNoRepositoryCalls(repoMock);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowValidationException_WhenTitleExceedsMaxLength()
        {
            var request = BuildRequest();
            request.Title = new string('A', 201);
            var repoMock = new Mock<ITaskRepository>(MockBehavior.Strict);
            var service = new TaskService(repoMock.Object);

            var exception = await Assert.ThrowsAsync<ValidationException>(
                () => service.CreateAsync(request, CancellationToken.None));

            Assert.Equal("Title must be 200 characters or fewer", exception.Message);
            VerifyNoRepositoryCalls(repoMock);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(999)]
        public async Task CreateAsync_ShouldThrowValidationException_WhenStatusIsInvalid(int status)
        {
            var request = BuildRequest();
            request.Status = (DomainTaskStatus)status;
            var repoMock = new Mock<ITaskRepository>(MockBehavior.Strict);
            var service = new TaskService(repoMock.Object);

            var exception = await Assert.ThrowsAsync<ValidationException>(
                () => service.CreateAsync(request, CancellationToken.None));

            Assert.Equal("Status must be one of: New, InProgress, Done.", exception.Message);
            VerifyNoRepositoryCalls(repoMock);
        }

        private static CreateTaskRequestDto BuildRequest() => new()
        {
            ProjectId = 7,
            Title = "Implement unit tests",
            Description = "Cover create validation for TaskService",
            Status = DomainTaskStatus.InProgress
        };

        private static void VerifyNoRepositoryCalls(Mock<ITaskRepository> repoMock)
        {
            repoMock.Verify(r => r.ProjectExistsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
            repoMock.Verify(r => r.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()), Times.Never);
            repoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            repoMock.VerifyNoOtherCalls();
        }
    }
}

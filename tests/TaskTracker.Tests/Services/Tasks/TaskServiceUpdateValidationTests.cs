using Moq;
using TaskTracker.Application.Dtos;
using TaskTracker.Application.Exceptions;
using TaskTracker.Application.Services;
using DomainTaskStatus = TaskTracker.Domain.Models.TaskStatus;

namespace TaskTracker.Tests.Services.Tasks
{
    public class TaskServiceUpdateValidationTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task UpdateAsync_ShouldThrowValidationException_WhenProjectIdIsNotPositive(int projectId)
        {
            var request = BuildRequest();
            request.ProjectId = projectId;

            var taskRepoMock = new Mock<ITaskRepository>(MockBehavior.Strict);
            var projectRepoMock = new Mock<IProjectRepository>(MockBehavior.Strict);
            var clockMock = new Mock<IClock>(MockBehavior.Strict);
            var service = new TaskService(taskRepoMock.Object, projectRepoMock.Object, clockMock.Object);

            var exception = await Assert.ThrowsAsync<ValidationException>(
                () => service.UpdateAsync(33, request, CancellationToken.None));

            Assert.Equal("ProjectId must be a positive number.", exception.Message);
            VerifyNoDependencyCalls(taskRepoMock, projectRepoMock, clockMock);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task UpdateAsync_ShouldThrowValidationException_WhenTitleIsNullEmptyOrWhiteSpace(string? title)
        {
            var request = BuildRequest();
            request.Title = title;

            var taskRepoMock = new Mock<ITaskRepository>(MockBehavior.Strict);
            var projectRepoMock = new Mock<IProjectRepository>(MockBehavior.Strict);
            var clockMock = new Mock<IClock>(MockBehavior.Strict);
            var service = new TaskService(taskRepoMock.Object, projectRepoMock.Object, clockMock.Object);

            var exception = await Assert.ThrowsAsync<ValidationException>(
                () => service.UpdateAsync(22, request, CancellationToken.None));

            Assert.Equal("Title is required.", exception.Message);
            VerifyNoDependencyCalls(taskRepoMock, projectRepoMock, clockMock);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowValidationException_WhenTitleExceedsMaxLength()
        {
            var request = BuildRequest();
            request.Title = new string('a', 210);

            var taskRepoMock = new Mock<ITaskRepository>(MockBehavior.Strict);
            var projectRepoMock = new Mock<IProjectRepository>(MockBehavior.Strict);
            var clockMock = new Mock<IClock>(MockBehavior.Strict);
            var service = new TaskService(taskRepoMock.Object, projectRepoMock.Object, clockMock.Object);

            var exception = await Assert.ThrowsAsync<ValidationException>(
                () => service.UpdateAsync(22, request, CancellationToken.None));

            Assert.Equal("Title must be 200 characters or fewer.", exception.Message);
            VerifyNoDependencyCalls(taskRepoMock, projectRepoMock, clockMock);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(999)]
        public async Task UpdateAsync_ShouldThrowValidationException_WhenStatusIsInvalid(int status)
        {
            var request = BuildRequest();
            request.Status = (DomainTaskStatus)status;

            var taskRepoMock = new Mock<ITaskRepository>(MockBehavior.Strict);
            var projectRepoMock = new Mock<IProjectRepository>(MockBehavior.Strict);
            var clockMock = new Mock<IClock>(MockBehavior.Strict);
            var service = new TaskService(taskRepoMock.Object, projectRepoMock.Object, clockMock.Object);

            var exception = await Assert.ThrowsAsync<ValidationException>(
                () => service.UpdateAsync(22, request, CancellationToken.None));

            Assert.Equal("Status must be one of: New, InProgress, Done", exception.Message);
            VerifyNoDependencyCalls(taskRepoMock, projectRepoMock, clockMock);
        }

        private static UpdateTaskRequestDto BuildRequest() => new()
        {
            ProjectId = 12,
            Title = "Ship update flow",
            Description = "Cover update validation for TaskService",
            Status = DomainTaskStatus.Done
        };

        private static void VerifyNoDependencyCalls(
            Mock<ITaskRepository> taskRepoMock,
            Mock<IProjectRepository> projectRepoMock,
            Mock<IClock> clockMock)
        {
            taskRepoMock.VerifyNoOtherCalls();
            projectRepoMock.VerifyNoOtherCalls();
            clockMock.VerifyNoOtherCalls();
        }
    }
}

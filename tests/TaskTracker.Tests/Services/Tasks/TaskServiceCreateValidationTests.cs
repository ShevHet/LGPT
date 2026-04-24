using Moq;
using TaskTracker.Application.Dtos;
using TaskTracker.Application.Exceptions;
using TaskTracker.Application.Services;
using DomainTaskStatus = TaskTracker.Domain.Models.TaskStatus;
using static TaskTracker.Tests.Services.Tasks.TaskServiceTestHelpers;

namespace TaskTracker.Tests.Services.Tasks
{
    public class TaskServiceCreateValidationTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task CreateAsync_ShouldThrowValidationException_WhenProjectIdIsNotPositive(int projectId)
        {
            var request = BuildCreateRequest();
            request.ProjectId = projectId;

            var service = CreateServiceWithStrictMocks(
                out var taskRepoMock,
                out var projectRepoMock,
                out var clockMock);

            var exception = await Assert.ThrowsAsync<ValidationException>(
                () => service.CreateAsync(request, CancellationToken.None));

            Assert.Equal("ProjectId must be a positive number.", exception.Message);
            VerifyNoDependencyCalls(taskRepoMock, projectRepoMock, clockMock);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task CreateAsync_ShouldThrowValidationException_WhenTitleIsNullEmptyOrWhiteSpace(string? title)
        {
            var request = BuildCreateRequest();
            request.Title = title;

            var service = CreateServiceWithStrictMocks(
                out var taskRepoMock,
                out var projectRepoMock,
                out var clockMock);

            var exception = await Assert.ThrowsAsync<ValidationException>(
                () => service.CreateAsync(request, CancellationToken.None));

            Assert.Equal("Title is required.", exception.Message);
            VerifyNoDependencyCalls(taskRepoMock, projectRepoMock, clockMock);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowValidationException_WhenTitleExceedsMaxLength()
        {
            var request = BuildCreateRequest();
            request.Title = new string('A', 201);

            var service = CreateServiceWithStrictMocks(
                out var taskRepoMock,
                out var projectRepoMock,
                out var clockMock);

            var exception = await Assert.ThrowsAsync<ValidationException>(
                () => service.CreateAsync(request, CancellationToken.None));

            Assert.Equal("Title must be 200 characters or fewer.", exception.Message);
            VerifyNoDependencyCalls(taskRepoMock, projectRepoMock, clockMock);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(999)]
        public async Task CreateAsync_ShouldThrowValidationException_WhenStatusIsInvalid(int status)
        {
            var request = BuildCreateRequest();
            request.Status = (DomainTaskStatus)status;

            var service = CreateServiceWithStrictMocks(
                out var taskRepoMock,
                out var projectRepoMock,
                out var clockMock);

            var exception = await Assert.ThrowsAsync<ValidationException>(
                () => service.CreateAsync(request, CancellationToken.None));

            Assert.Equal("Status must be one of: New, InProgress, Done", exception.Message);
            VerifyNoDependencyCalls(taskRepoMock, projectRepoMock, clockMock);
        }

        private static CreateTaskRequestDto BuildCreateRequest() => new()
        {
            ProjectId = 7,
            Title = "Implement unit tests",
            Description = "Cover create validation for TaskService",
            Status = DomainTaskStatus.InProgress
        };

        private static TaskService CreateServiceWithStrictMocks(
            out Mock<ITaskRepository> taskRepoMock,
            out Mock<IProjectRepository> projectRepoMock,
            out Mock<IClock> clockMock)
        {
            taskRepoMock = CreateStrictTaskRepositoryMock();
            projectRepoMock = CreateStrictProjectRepositoryMock();
            clockMock = CreateStrictClockMock();
            return CreateService(taskRepoMock, projectRepoMock, clockMock);
        }
    }
}

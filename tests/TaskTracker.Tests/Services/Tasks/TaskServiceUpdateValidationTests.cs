using System.ComponentModel.DataAnnotations;
using System.Net.WebSockets;
using System.Windows.Markup;
using Moq;
using TaskTracker.Application.Dtos;
using TaskTracker.Application.Services;
using TaskTracker.Domain.Models;
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
            var repoMock = new Mock<ITaskRepository>(MockBehavior.Strict);
            var service = new TaskService(repoMock.Object);

            var exception = await Assert.ThrowsAsync<ValidationException>(
                () => service.UpdateAsync(33, request, CancellationToken.None));

            Assert.Equal("Project id must be a positive number", exception.Message);
            VerifyNoRepositoryCalls(repoMock);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task UpdateAsync_ShouldThrowValidationException_WhenTitleEmmptyOrWriteSpace(string? title)
        {
            var request = BuildRequest();
            request.Title = title;
            var repoMock = new Mock<ITaskRepository>( MockBehavior.Strict);
            var service = new TaskService(repoMock.Object);

            var exception = await Assert.ThrowsAsync<ValidationException>(
                ()=> service.UpdateAsync(22, request, CancellationToken.None));

            Assert.Equal("Title is required", exception.Message);
            VerifyNoRepositoryCalls(repoMock);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowValidationException_WhenProjectIdExceedsMaxLengh()
        {
            var request = BuildRequest();
            request.Title = new string('a', 210);
            var repoMock = new  Mock<ITaskRepository>(MockBehavior.Strict);
            var service = new TaskService(repoMock.Object);

            var exception = await Assert.ThrowsAsync<ValidationException>(
                () => service.UpdateAsync(22, request, CancellationToken.None));

            Assert.Equal("Title must be 200 characters or fewer.", exception.Message);
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
                () => service.UpdateAsync(22, request, CancellationToken.None));

            Assert.Equal("Status must be one of: New, InProgress, Done.", exception.Message);
            VerifyNoRepositoryCalls(repoMock);
        }

        private static UpdateTaskRequestDto BuildRequest() => new()
        {
            ProjectId = 12,
            Title = "Ship update flow",
            Description = "Cover update validation for TaskService",
            Status = DomainTaskStatus.Done
        };

        private static void VerifyNoRepositoryCalls(Mock<ITaskRepository> repoMock)
        {
            repoMock.Verify(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
            repoMock.Verify(r => r.ProjectExistsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
            repoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            repoMock.VerifyNoOtherCalls();
        }
    }
}

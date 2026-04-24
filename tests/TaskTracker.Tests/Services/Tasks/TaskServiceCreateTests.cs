using Moq;
using TaskTracker.Application.Dtos;
using TaskTracker.Application.Services;
using TaskTracker.Domain.Models;
using DomainTaskStatus = TaskTracker.Domain.Models.TaskStatus;

namespace TaskTracker.Tests.Services.Tasks
{
    public class TaskServiceCreateTests
    {
        [Fact]
        public async Task CreateAsync_ShouldReturnCreatedTaskResponse_WhenRequestIsValid()
        {
            var request = BuildRequest();
            var fixedUtcNow = new DateTime(2026, 04, 21, 12, 0, 0, DateTimeKind.Utc);
            var taskRepoMock = CreateTaskRepositoryMockForSuccessfulCreate("Website Redesign", 101);
            var projectRepoMock = CreateProjectRepositoryMockForExistingProject(request.ProjectId);
            var clockMock = CreateClockMock(fixedUtcNow);
            var service = new TaskService(taskRepoMock.Object, projectRepoMock.Object, clockMock.Object);

            var result = await service.CreateAsync(request, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(101, result.Id);
            Assert.Equal(request.ProjectId, result.ProjectId);
            Assert.Equal("Website Redesign", result.ProjectName);
            Assert.Equal(fixedUtcNow, result.CreatedAt);
            Assert.Equal(fixedUtcNow, result.UpdatedAt);
        }

        [Fact]
        public async Task CreateAsync_ShouldMapRequestFieldsToTaskAndResponse_WhenRequestIsValid()
        {
            var request = BuildRequest();
            var fixedUtcNow = new DateTime(2026, 04, 21, 12, 0, 0, DateTimeKind.Utc);
            TaskItem? capturedTask = null;

            var taskRepoMock = new Mock<ITaskRepository>(MockBehavior.Strict);
            var projectRepoMock = CreateProjectRepositoryMockForExistingProject(request.ProjectId);
            var clockMock = CreateClockMock(fixedUtcNow);

            taskRepoMock
                .Setup(repository => repository.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
                .Callback<TaskItem, CancellationToken>((task, _) =>
                {
                    capturedTask = task;
                    task.Id = 55;
                    task.Project = new Project
                    {
                        Id = request.ProjectId,
                        Name = "Mobile App"
                    };
                })
                .Returns(Task.CompletedTask);

            taskRepoMock
                .Setup(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = new TaskService(taskRepoMock.Object, projectRepoMock.Object, clockMock.Object);

            var result = await service.CreateAsync(request, CancellationToken.None);

            Assert.NotNull(capturedTask);
            Assert.Equal(request.ProjectId, capturedTask.ProjectId);
            Assert.Equal(request.Title, capturedTask.Title);
            Assert.Equal(request.Description, capturedTask.Description);
            Assert.Equal(request.Status, capturedTask.Status);
            Assert.Equal(fixedUtcNow, capturedTask.CreatedAt);
            Assert.Equal(fixedUtcNow, capturedTask.UpdatedAt);

            Assert.Equal(request.ProjectId, result.ProjectId);
            Assert.Equal(request.Title, result.Title);
            Assert.Equal(request.Description, result.Description);
            Assert.Equal(request.Status, result.Status);
            Assert.Equal("Mobile App", result.ProjectName);
            Assert.Equal(fixedUtcNow, result.CreatedAt);
            Assert.Equal(fixedUtcNow, result.UpdatedAt);
        }

        [Fact]
        public async Task CreateAsync_ShouldCallAddAndSaveOnce_WhenRequestIsValid()
        {
            var request = BuildRequest();
            var fixedUtcNow = new DateTime(2026, 04, 21, 12, 0, 0, DateTimeKind.Utc);
            var taskRepoMock = CreateTaskRepositoryMockForSuccessfulCreate("Data Platform");
            var projectRepoMock = CreateProjectRepositoryMockForExistingProject(request.ProjectId);
            var clockMock = CreateClockMock(fixedUtcNow);
            var service = new TaskService(taskRepoMock.Object, projectRepoMock.Object, clockMock.Object);

            await service.CreateAsync(request, CancellationToken.None);

            projectRepoMock.Verify(repository => repository.ExistsAsync(request.ProjectId, It.IsAny<CancellationToken>()), Times.Once);
            clockMock.VerifyGet(clock => clock.UtcNow, Times.Once);
            taskRepoMock.Verify(repository => repository.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()), Times.Once);
            taskRepoMock.Verify(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            taskRepoMock.VerifyNoOtherCalls();
            projectRepoMock.VerifyNoOtherCalls();
            clockMock.VerifyNoOtherCalls();
        }

        private static CreateTaskRequestDto BuildRequest() => new()
        {
            ProjectId = 7,
            Title = "Implement unit tests",
            Description = "Cover create happy-path for TaskService",
            Status = DomainTaskStatus.InProgress
        };

        private static Mock<ITaskRepository> CreateTaskRepositoryMockForSuccessfulCreate(
            string projectName,
            int generatedTaskId = 1)
        {
            var taskRepoMock = new Mock<ITaskRepository>(MockBehavior.Strict);

            taskRepoMock
                .Setup(repository => repository.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
                .Callback<TaskItem, CancellationToken>((task, _) =>
                {
                    task.Id = generatedTaskId;
                    task.Project = new Project
                    {
                        Id = task.ProjectId,
                        Name = projectName
                    };
                })
                .Returns(Task.CompletedTask);

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

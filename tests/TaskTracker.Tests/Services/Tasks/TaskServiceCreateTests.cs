using Moq;
using TaskTracker.Application.Dtos;
using TaskTracker.Application.Services;
using TaskTracker.Domain.Models;
using DomainTaskStatus = TaskTracker.Domain.Models.TaskStatus;
using static TaskTracker.Tests.Services.Tasks.TaskServiceTestHelpers;

namespace TaskTracker.Tests.Services.Tasks
{
    public class TaskServiceCreateTests
    {
        [Fact]
        public async Task CreateAsync_ShouldReturnCreatedTaskResponse_WhenRequestIsValid()
        {
            var request = BuildCreateRequest();
            var taskRepoMock = CreateTaskRepositoryMockForSuccessfulCreate("Website Redesign", 101);
            var projectRepoMock = CreateProjectRepositoryMockWithExistsResult(request.ProjectId, true);
            var clockMock = CreateClockMock(FixedUtcNow);
            var service = CreateService(taskRepoMock, projectRepoMock, clockMock);

            var result = await service.CreateAsync(request, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(101, result.Id);
            Assert.Equal(request.ProjectId, result.ProjectId);
            Assert.Equal("Website Redesign", result.ProjectName);
            Assert.Equal(FixedUtcNow, result.CreatedAt);
            Assert.Equal(FixedUtcNow, result.UpdatedAt);
        }

        [Fact]
        public async Task CreateAsync_ShouldMapRequestFieldsToTaskAndResponse_WhenRequestIsValid()
        {
            var request = BuildCreateRequest();
            TaskItem? capturedTask = null;

            var taskRepoMock = CreateStrictTaskRepositoryMock();
            var projectRepoMock = CreateProjectRepositoryMockWithExistsResult(request.ProjectId, true);
            var clockMock = CreateClockMock(FixedUtcNow);

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

            var service = CreateService(taskRepoMock, projectRepoMock, clockMock);

            var result = await service.CreateAsync(request, CancellationToken.None);

            Assert.NotNull(capturedTask);
            Assert.Equal(request.ProjectId, capturedTask.ProjectId);
            Assert.Equal(request.Title, capturedTask.Title);
            Assert.Equal(request.Description, capturedTask.Description);
            Assert.Equal(request.Status, capturedTask.Status);
            Assert.Equal(FixedUtcNow, capturedTask.CreatedAt);
            Assert.Equal(FixedUtcNow, capturedTask.UpdatedAt);

            Assert.Equal(request.ProjectId, result.ProjectId);
            Assert.Equal(request.Title, result.Title);
            Assert.Equal(request.Description, result.Description);
            Assert.Equal(request.Status, result.Status);
            Assert.Equal("Mobile App", result.ProjectName);
            Assert.Equal(FixedUtcNow, result.CreatedAt);
            Assert.Equal(FixedUtcNow, result.UpdatedAt);
        }

        [Fact]
        public async Task CreateAsync_ShouldCallAddAndSaveOnce_WhenRequestIsValid()
        {
            var request = BuildCreateRequest();
            var taskRepoMock = CreateTaskRepositoryMockForSuccessfulCreate("Data Platform");
            var projectRepoMock = CreateProjectRepositoryMockWithExistsResult(request.ProjectId, true);
            var clockMock = CreateClockMock(FixedUtcNow);
            var service = CreateService(taskRepoMock, projectRepoMock, clockMock);

            await service.CreateAsync(request, CancellationToken.None);

            projectRepoMock.Verify(repository => repository.ExistsAsync(request.ProjectId, It.IsAny<CancellationToken>()), Times.Once);
            clockMock.VerifyGet(clock => clock.UtcNow, Times.Once);
            taskRepoMock.Verify(repository => repository.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()), Times.Once);
            taskRepoMock.Verify(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            VerifyNoDependencyCalls(taskRepoMock, projectRepoMock, clockMock);
        }

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
    }
}

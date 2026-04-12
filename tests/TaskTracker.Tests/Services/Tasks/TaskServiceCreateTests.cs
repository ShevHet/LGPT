using Moq;
using Newtonsoft.Json.Serialization;
using System.ComponentModel.DataAnnotations;
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
            var repoMock = CreateRepositoryMockForSuccessfulCreate(request.ProjectId, "Website Redesign", 101);
            var service = new TaskService(repoMock.Object);

            var result = await service.CreateAsync(request,CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(101, result.Id);
            Assert.Equal(request.ProjectId, result.ProjectId);
            Assert.Equal("Website Redesign", result.ProjectName);
            Assert.NotEqual(default, result.CreatedAt);
            Assert.NotEqual(default, result.UpdatedAt);
        }

        [Fact]
        public async Task CreateAsync_ShouldMapRequestFieldsToTaskAndResponse_WhenRequestIsValid()
        {
            var request = BuildRequest();
            TaskItem? capturedTask = null;

            var repoMock = new Mock<ITaskRepository>(MockBehavior.Strict);

            repoMock
                .Setup(r => r.ProjectExistsAsync(request.ProjectId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            repoMock
                .Setup(r => r.ProjectExistsAsync(request.ProjectId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            repoMock
                .Setup(r => r.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
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

            repoMock
                .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = new TaskService(repoMock.Object);

            var result = await service.CreateAsync(request, CancellationToken.None);

            Assert.NotNull(capturedTask);
            Assert.Equal(request.ProjectId, capturedTask.ProjectId);
            Assert.Equal(request.Title, capturedTask.Title);
            Assert.Equal(request.Description, capturedTask.Description);
            Assert.Equal(request.Status, capturedTask.Status);
            Assert.NotEqual(default, capturedTask.CreatedAt);
            Assert.NotEqual(default, capturedTask.UpdatedAt);

            Assert.Equal(request.ProjectId, result.ProjectId);
            Assert.Equal(request.Title, result.Title);
            Assert.Equal(request.Description, result.Description);
            Assert.Equal(request.Status, result.Status);
            Assert.Equal("Mobile App", result.ProjectName);
        }

        [Fact]
        public async Task CreateAsync_ShouldCallAddAndSaveOnce_WhenRequestIsValid()
        {
            //Arrange
            var request = BuildRequest();
            var repoMock = CreateRepositoryMockForSuccessfulCreate(request.ProjectId, "Data Platform");
            var service = new TaskService(repoMock.Object);

            //Act
            await service.CreateAsync(request, CancellationToken.None);

            //Assert
            repoMock.Verify(r => r.ProjectExistsAsync(request.ProjectId, It.IsAny<CancellationToken>()), Times.Once);
            repoMock.Verify(r=>r.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()), Times.Once);
            repoMock.Verify(r=>r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            repoMock.VerifyNoOtherCalls();
        }

        private static CreateTaskRequestDto BuildRequest() => new()
        {
            ProjectId = 7,
            Title = "Implement unit tests",
            Description = "Cover create happy-path for TaskService",
            Status = DomainTaskStatus.InProgress
        };

        private static Mock<ITaskRepository> CreateRepositoryMockForSuccessfulCreate(
            int projectId,
            string projectName,
            int generatedTaskId = 1)
        {
            var repoMock = new Mock<ITaskRepository>(MockBehavior.Strict);

            repoMock
                .Setup(r => r.ProjectExistsAsync(projectId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            repoMock
                .Setup(r => r.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
               .Callback<TaskItem, CancellationToken>((task, _) =>
               {
                   task.Id = generatedTaskId;
                   task.Project = new Project
                   {
                       Id = projectId,
                       Name = projectName
                   };
               })
               .Returns(Task.CompletedTask);

            repoMock
                .Setup(r=>r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            return repoMock;
        }
    }
}

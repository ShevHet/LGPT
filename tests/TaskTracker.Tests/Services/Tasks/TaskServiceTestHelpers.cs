using System.ComponentModel;
using Moq;
using TaskTracker.Application.Dtos;
using TaskTracker.Application.Services;
using TaskTracker.Domain.Models;
using DomainTaskStatus = TaskTracker.Domain.Models.TaskStatus;

namespace TaskTracker.Tests.Services.Tasks
{
    public class TaskServiceTestHelpers
    {
        public static readonly DateTime FixedUtcNow = new(2026, 04, 21, 12, 0, 0, DateTimeKind.Utc);;

        public static CreateTaskRequestDto BuildCreateRequest(
            int projectId = 7,
            string title = "Implement unit tests",
            string? desctiption = "Cover create flow for TaskService",
            DomainTaskStatus status = DomainTaskStatus.InProgress) => new()
            {
                ProjectId = projectId,
                Title = title,
                Description = desctiption,
                Status = status
            };

        public static UpdateTaskRequestDto BuildUpdateRequest(
            int projectId = 12,
            string tite = "Ship update flow",
            string? description = "Refresh tasl fields from dto",
            DomainTaskStatus status = DomainTaskStatus.Done) => new()
            {
                ProjectId = projectId,
                Title = tite,
                Description = description,
                Status = status
            };

        public static TaskItem BuildExistingTask(
            int id = 42,
            int projectId = 7,
            string projectName = "Website Redesign",
            string title = "Draft API contract",
            string? description = "Initial version",
            DomainTaskStatus status = DomainTaskStatus.New) => new()
            {
                Id = id,
                ProjectId = projectId,
                Title = title,
                Description = description,
                Status = status,
                CreatedAt = new DateTime(2026, 04, 10, 8, 30, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2026, 04, 11, 9, 0, 0, DateTimeKind.Utc),
                Project = BuildProject(projectId, projectName)
            };

        public static Project BuildProject(int id, string name) => new()
        {
            Id = id,
            Name = name
        };

        public static Mock<ITaskRepository> CreateStrictTaskRepositoryMock()
            => new(MockBehavior.Strict);

        public static Mock<IProjectRepository> CreateStrictProjectRepositoryMock()
            => new(MockBehavior.Strict);

        public static Mock<IClock> CreateStrictClockMock()
            => new(MockBehavior.Strict);

        public static Mock<IProjectRepository> CreateProjectRepositoryMockWithExistsResult(
            int projectId,
            bool exists)
        {
            var projectRepoMock = CreateStrictProjectRepositoryMock();

            projectRepoMock
                .Setup(r=>r.ExistsAsync(projectId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(exists);

            return projectRepoMock;
        }

        public static Mock<IClock> CreateClockMock(DateTime utcNow)
        {
            var clockMock = new Mock<IClock>();

            clockMock
                .SetupGet(c=>c.UtcNow)
                .Returns(utcNow);

            return clockMock;
        }

        public static TaskService CreateService(
            Mock<ITaskRepository> taskRepoMock,
            Mock<IProjectRepository> projectRepoMock,
            Mock<IClock> clockRepoMock) =>
            new(taskRepoMock.Object, projectRepoMock.Object, clockRepoMock.Object);

        public static void VerifyNoDependencyCalls(
            Mock<ITaskRepository> taskRepoMock,
            Mock<IProjectRepository> projectRepoMock,
            Mock<IClock> clockRepoMock)
        {
            taskRepoMock.VerifyNoOtherCalls();
            projectRepoMock.VerifyNoOtherCalls();
            clockRepoMock.VerifyNoOtherCalls();
        }
    }
}

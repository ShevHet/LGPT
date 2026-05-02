using System.Net;
using System.Net.Http.Json;
using TaskTracker.Application.Dtos;

namespace TaskTracker.Tests.Integration
{
    public sealed class TasksApiHappyPath : IClassFixture<ApiFactory>
    {
        private readonly ApiFactory _factory;

        public TasksApiHappyPath(ApiFactory factory) =>
            _factory = factory;

        [Fact]
        public async Task TaskLifecycle_ShouldCreateGetAndDeleteTask()
        {
            await _factory.ResetDatabaseAsync();
            using var client = _factory.CreateClient();

            var projectResponse = await client.PostAsJsonAsync("/projects", new
            {
                name = "Integration Test Project"
            });

            Assert.Equal(HttpStatusCode.Created, projectResponse.StatusCode);

            var project = await projectResponse.Content.ReadFromJsonAsync<ProjectResponse>();

            Assert.NotNull(project);
            Assert.True(project!.Id > 0);

            var createTaskResponse = await client.PostAsJsonAsync("/tasks", new
            {
                projectId = project.Id,
                title = "Integration Test Task",
                description = "Created from integration test",
                status = "New"
            });
            Assert.Equal(HttpStatusCode.Created, createTaskResponse.StatusCode);

            var task = await createTaskResponse.Content.ReadFromJsonAsync<TaskResponse>();

            Assert.NotNull(task);
            Assert.True(task!.Id > 0);
            Assert.Equal(project.Id, task.ProjectId);
            Assert.Equal("Integration Test Task", task.Title);
            Assert.Equal("Created from integration test", task.Description);
            Assert.Equal("New", task.Status);

            var getResponse = await client.GetAsync($"/tasks/{task.Id}");

            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var fetched = await getResponse.Content.ReadFromJsonAsync<TaskResponse>();

            Assert.NotNull(fetched);
            Assert.Equal(task.Id, fetched!.Id);
            Assert.Equal(project.Id, fetched.ProjectId);
            Assert.Equal("Integration Test Task", fetched.Title);

            var deleteResponse = await client.DeleteAsync($"/tasks/{task.Id}");

            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        }

        private sealed class ProjectResponse
        {
            public int Id { get; set; }
        }

        private sealed class TaskResponse
        {
            public int Id { get; set; }
            public int ProjectId { get; set; }
            public string Title { get; set; }
            public string? Description { get; set; }
            public string Status {  get; set; } = string.Empty;
        }
    }
}

using System.Net;
using System.Net.Http.Json;

namespace TaskTracker.Tests.Integration
{
    public sealed class ProjectApiErrorTests : IClassFixture<ApiFactory>
    {
        private readonly ApiFactory _factory;
        private readonly HttpClient _client;

        public ProjectApiErrorTests(ApiFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetProjectById_ShouldReturnBadRequest_WhenIdNotPositive()
        {
            await _factory.ResetDatabaseAsync();

            var response = await _client.GetAsync("/projects/0");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();

            Assert.NotNull(error);
            Assert.False(string.IsNullOrWhiteSpace(error!.TraceId));
            Assert.False(string.IsNullOrWhiteSpace(error.Message));
            Assert.Null(error.Errors);
        }

        [Fact]
        public async Task GetProjectById_ShouldReturnNotFound_WhenProjectDoesNotExists()
        {
            await _factory.ResetDatabaseAsync();

            var response = await _client.GetAsync("/projects/999999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();

            Assert.NotNull(error);
            Assert.False(string.IsNullOrWhiteSpace(error!.TraceId));
            Assert.False(string.IsNullOrWhiteSpace(error.Message));
            Assert.Null(error.Errors);
        }

        [Fact]
        public async Task DeleteProject_ShouldReturnApiErrorResponse_WhenProjectHasTasks()
        {
            await _factory.ResetDatabaseAsync();

            var createdProjectResponse = await _client.PostAsJsonAsync("/projects", new
            {
                name = "Project With Task"
            });

            Assert.Equal(HttpStatusCode.Created, createdProjectResponse.StatusCode);

            var project = await createdProjectResponse.Content.ReadFromJsonAsync<ProjectResponse>();

            Assert.NotNull(project);

            var createdTaskResponse = await _client.PostAsJsonAsync("/tasks", new
            {
                projectId = project!.Id,
                title = "Task blocking project delete",
                description = "This task should cause conflict",
                status = "New"
            });

            Assert.Equal(HttpStatusCode.Created, createdTaskResponse.StatusCode);

            var deleteResponse = await _client.DeleteAsync($"/projects/{project.Id}");

            Assert.Equal(HttpStatusCode.Conflict, deleteResponse.StatusCode);

            var error = await deleteResponse.Content.ReadFromJsonAsync<ApiErrorResponse>();

            Assert.NotNull(error);
            Assert.False(string.IsNullOrWhiteSpace(error!.TraceId));
            Assert.False(string.IsNullOrWhiteSpace(error.Message));
            Assert.Null(error.Errors);
        }

        private sealed record ApiErrorResponse(
            string TraceId,
            string Message,
            Dictionary<string, string[]>? Errors);

        private sealed record ProjectResponse(int Id, string Name);
    }
}

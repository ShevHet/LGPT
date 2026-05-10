using System.Net;
using System.Net.Http.Json;

namespace TaskTracker.Tests.Integration
{
    public sealed class TaskApiErrorTests : IClassFixture<ApiFactory>
    {
        private readonly ApiFactory _factory;
        private readonly HttpClient _client;

        public TaskApiErrorTests(ApiFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateTask_ShouldReturnBadRequest_WhenRequestIsInvalid()
        {
            await _factory.ResetDatabaseAsync();

            var response = await _client.PostAsJsonAsync("/tasks", new
            {
                projectId = 0,
                title = "",
                description = "Invalid task",
                status = "New"
            });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();

            Assert.NotNull(error);
            Assert.False(string.IsNullOrWhiteSpace(error!.TraceId));
            Assert.Equal("Validation failed", error.Message);
            Assert.NotNull(error.Errors);
            Assert.NotEmpty(error.Errors);
        }

        [Fact]
        public async Task GetTaskById_ShouldReturnNotFound_WhenTaskDoesNotExist()
        {
            await _factory.ResetDatabaseAsync();

            var response = await _client.GetAsync("/tasks/999999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        private sealed record ApiErrorResponse(
            string TraceId,
            string Message,
            Dictionary<string, string[]>? Errors);
    }
}

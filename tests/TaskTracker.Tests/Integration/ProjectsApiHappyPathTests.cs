using System.Net;
using System.Net.Http.Json;
using System.Runtime.Intrinsics.X86;
using TaskTracker.Infrastructure.Services;

namespace TaskTracker.Tests.Integration
{
    public sealed class ProjectsApiHappyPathTests : IClassFixture<ApiFactory>
    {
        private readonly ApiFactory _factory;

        public ProjectsApiHappyPathTests(ApiFactory factory) =>
            _factory = factory;

        [Fact]
        public async Task ProjectLifecycle_ShouldCreateGetAndDeleteProject()
        {
            await _factory.ResetDatabaseAsync();
            using var client = _factory.CreateClient();

            const string projectName = "Integration Test Project";

            var createResponse = await client.PostAsJsonAsync("/projects",
                new { projectName = projectName });

            Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);

            var created = await createResponse.Content.ReadFromJsonAsync<ProjectResponse>();

            Assert.NotNull(created);
            Assert.True(created.Id > 0);
            Assert.Equal(projectName, created.Name);

            var getResponse = await client.GetAsync($"/projects/{created.Id}");

            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var fetched = await getResponse.Content.ReadFromJsonAsync<ProjectResponse>();

            Assert.NotNull(fetched);
            Assert.Equal(created.Id, fetched!.Id);
            Assert.Equal(projectName, fetched!.Name);

            var deleteResponse = await client.DeleteAsync($"/projects/{created.Id}");

            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        }

        private sealed class ProjectResponse
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}

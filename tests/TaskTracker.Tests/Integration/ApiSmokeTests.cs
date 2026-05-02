using System.Net;

namespace TaskTracker.Tests.Integration
{
    public class ApiSmokeTests : IClassFixture<ApiFactory>
    {
        private readonly HttpClient _client;

        public ApiSmokeTests(ApiFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task SwaggerJson_ShouldReturnSuccessStatusCode()
        {
            var response = await _client.GetAsync("/swagger/v1/swagger.json");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}

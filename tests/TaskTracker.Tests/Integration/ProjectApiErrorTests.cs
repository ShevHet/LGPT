<<<<<<< HEAD
﻿namespace TaskTracker.Tests.Integration
{
    public class ProjectApiErrorTests
    {
=======
﻿using System.Net;

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

            var response = await _client.GetAsync($"/projects/0");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetProjectById_ShouldReturnNotFound_WhenProjectDoesNotExists()
        {
            await _factory.ResetDatabaseAsync();

            var response = await _client.GetAsync("/projects/999999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
>>>>>>> 2244c33 (Добавлены интеграционные API-тесты для 400 и 404)
    }
}

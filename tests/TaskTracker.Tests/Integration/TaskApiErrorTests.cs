<<<<<<< HEAD
﻿namespace TaskTracker.Tests.Integration
{
    public class TaskApiErrorTests
    {
=======
﻿using System.Net;
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
            _client = new HttpClient();
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
        }

        [Fact]
        public async Task GetTaskById_ShouldReturnNotFound_WhenTaskDoesNotExist()
        {
            await _factory.ResetDatabaseAsync();

            var response = await _client.GetAsync("/tasks/999999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
>>>>>>> 2244c33 (Добавлены интеграционные API-тесты для 400 и 404)
    }
}

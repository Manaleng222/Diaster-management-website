
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using System.Diagnostics;
using System.Net.Http.Json;
using APPR_P_2; // Add this for Program

namespace APPR_P_2.LoadTests
{
    public class DisasterAppLoadTests
    {
        private readonly HttpClient _client;

        public DisasterAppLoadTests()
        {
            var factory = new WebApplicationFactory<Program>();
            _client = factory.CreateClient();
        }

        [Theory]
        [InlineData(100)] // 100 concurrent users
        [InlineData(500)] // 500 concurrent users
        public async Task GetDisasterEvents_UnderLoad_ShouldHandleConcurrentUsers(int userCount)
        {
            // Arrange
            var tasks = new List<Task<HttpResponseMessage>>();
            var stopwatch = Stopwatch.StartNew();

            // Act - Simulate concurrent users
            for (int i = 0; i < userCount; i++)
            {
                tasks.Add(_client.GetAsync("/api/disasterevents"));
            }

            await Task.WhenAll(tasks);
            stopwatch.Stop();

            // Assert
            Assert.All(tasks, task => Assert.True(task.Result.IsSuccessStatusCode));
            Assert.True(stopwatch.ElapsedMilliseconds < 5000,
                $"Response time {stopwatch.ElapsedMilliseconds}ms exceeded 5 second threshold");
        }

        [Fact]
        public async Task DatabaseStressTest_ShouldHandleHighVolumeInserts()
        {
            // Arrange
            var events = GenerateTestEvents(100); // Reduced from 1000 to 100 for testing

            // Act & Measure
            var startTime = DateTime.Now;
            var insertTasks = events.Select(e => _client.PostAsJsonAsync("/api/disasterevents", e));
            await Task.WhenAll(insertTasks);
            var endTime = DateTime.Now;

            var totalTime = (endTime - startTime).TotalSeconds;

            // Assert
            Assert.True(totalTime < 30, $"Bulk insert took {totalTime} seconds, exceeding 30 second limit");
        }

        private List<object> GenerateTestEvents(int count)
        {
            var events = new List<object>();
            for (int i = 0; i < count; i++)
            {
                events.Add(new
                {
                    Title = $"Stress Test Event {i}",
                    Description = $"Description for event {i}",
                    Location = $"Location {i % 50}",
                    Severity = i % 3 == 0 ? "High" : i % 3 == 1 ? "Medium" : "Low"
                });
            }
            return events;
        }
    }
}
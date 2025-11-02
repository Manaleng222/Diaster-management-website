using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using System.Net.Http.Json;
using APPR_P_2.Models;
using APPR_P_2;

namespace APPR_P_2.IntegrationTests
{
    public class DisasterEventsIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public DisasterEventsIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task CreateDisasterEvent_ThenGetEvent_ShouldReturnCreatedEvent()
        {
            // Arrange
            var newEvent = new
            {
                title = "Integration Test Disaster",
                description = "Test Description",
                location = "Test Location",
                severity = "Medium"
            };

            // Act - Create event
            var createResponse = await _client.PostAsJsonAsync("/api/disasterevents", newEvent);
            createResponse.EnsureSuccessStatusCode();

            var createdEvent = await createResponse.Content.ReadFromJsonAsync<IncidentReport>();

            // Null check to fix the dereference error
            Assert.NotNull(createdEvent);
            Assert.NotNull(createdEvent.Id);

            // Act - Get event
            var getResponse = await _client.GetAsync($"/api/disasterevents/{createdEvent.Id}");
            getResponse.EnsureSuccessStatusCode();

            var retrievedEvent = await getResponse.Content.ReadFromJsonAsync<IncidentReport>();

            // Assert
            Assert.NotNull(retrievedEvent);
            Assert.Equal(newEvent.title, retrievedEvent.Title);
            Assert.Equal(newEvent.location, retrievedEvent.Location);
        }

        [Fact]
        public async Task DatabaseConnection_ShouldBeHealthy()
        {
            // Act
            var response = await _client.GetAsync("/health");

            // Assert
            response.EnsureSuccessStatusCode();
            var healthStatus = await response.Content.ReadAsStringAsync();
            Assert.Equal("Healthy", healthStatus);
        }
    }
}
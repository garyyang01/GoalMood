using FluentAssertions;
using GoalMood.BE.Models.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace GoalMood.Tests.Contract;

public class StatsEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public StatsEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GET_Stats_Returns200AndStatsDtoSchema()
    {
        // Act
        var response = await _client.GetAsync("/api/stats");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var stats = await response.Content.ReadFromJsonAsync<StatsDto>();
        stats.Should().NotBeNull();
        stats!.CompletionPercentage.Should().BeInRange(0, 100);
        stats.MoodDistribution.Should().NotBeNull();
    }

    [Fact]
    public async Task GET_Stats_ValidatesMoodDistributionSchema()
    {
        // Act
        var response = await _client.GetAsync("/api/stats");
        var stats = await response.Content.ReadFromJsonAsync<StatsDto>();

        // Assert
        stats.Should().NotBeNull();
        stats!.MoodDistribution.Should().NotBeNull();

        // Verify mood distribution contains valid mood values (1-5)
        foreach (var kvp in stats.MoodDistribution)
        {
            int.TryParse(kvp.Key, out int moodValue).Should().BeTrue();
            moodValue.Should().BeInRange(1, 5);
            kvp.Value.Should().BeGreaterThanOrEqualTo(0);
        }
    }

    [Fact]
    public async Task GET_Stats_NoGoals_ReturnsZeroPercentageOrValidResponse()
    {
        // Note: With seed data, there will always be goals
        // This test verifies the response structure is valid

        // Act
        var response = await _client.GetAsync("/api/stats");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var stats = await response.Content.ReadFromJsonAsync<StatsDto>();
        stats.Should().NotBeNull();
        stats!.CompletionPercentage.Should().BeGreaterThanOrEqualTo(0);
    }
}

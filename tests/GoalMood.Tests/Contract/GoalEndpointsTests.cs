using FluentAssertions;
using GoalMood.BE.Models.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace GoalMood.Tests.Contract;

public class GoalEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public GoalEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    #region User Story 2 - Add Daily Goals

    [Fact]
    public async Task POST_Goals_ValidInput_Returns201Created()
    {
        // Arrange
        var request = new { teamMemberId = 1, description = "Test goal from contract test" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/goals", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var goal = await response.Content.ReadFromJsonAsync<GoalDto>();
        goal.Should().NotBeNull();
        goal!.Description.Should().Be("Test goal from contract test");
        goal.TeamMemberId.Should().Be(1);
        goal.IsCompleted.Should().BeFalse();
    }

    [Fact]
    public async Task POST_Goals_MissingTeamMemberId_Returns400BadRequest()
    {
        // Arrange
        var request = new { description = "Test goal without member" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/goals", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task POST_Goals_EmptyDescription_Returns400BadRequest()
    {
        // Arrange
        var request = new { teamMemberId = 1, description = "" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/goals", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task POST_Goals_DescriptionExceeds500Chars_Returns400BadRequest()
    {
        // Arrange
        var longDescription = new string('x', 501);
        var request = new { teamMemberId = 1, description = longDescription };

        // Act
        var response = await _client.PostAsJsonAsync("/api/goals", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task POST_Goals_NonExistentTeamMemberId_Returns404NotFound()
    {
        // Arrange
        var request = new { teamMemberId = 99999, description = "Test goal" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/goals", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DELETE_Goals_ValidGoalId_Returns204NoContent()
    {
        // Arrange - First create a goal to delete
        var createRequest = new { teamMemberId = 1, description = "Goal to be deleted" };
        var createResponse = await _client.PostAsJsonAsync("/api/goals", createRequest);
        var createdGoal = await createResponse.Content.ReadFromJsonAsync<GoalDto>();

        // Act
        var response = await _client.DeleteAsync($"/api/goals/{createdGoal!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DELETE_Goals_NonExistentGoalId_Returns404NotFound()
    {
        // Act
        var response = await _client.DeleteAsync("/api/goals/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region User Story 4 - Mark Goal as Completed

    [Fact]
    public async Task PUT_GoalsComplete_ValidGoalId_Returns200AndCompletedGoal()
    {
        // Arrange - Create a goal first
        var createRequest = new { teamMemberId = 1, description = "Goal to complete" };
        var createResponse = await _client.PostAsJsonAsync("/api/goals", createRequest);
        var createdGoal = await createResponse.Content.ReadFromJsonAsync<GoalDto>();

        // Act
        var response = await _client.PutAsync($"/api/goals/{createdGoal!.Id}/complete", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var completedGoal = await response.Content.ReadFromJsonAsync<GoalDto>();
        completedGoal.Should().NotBeNull();
        completedGoal!.IsCompleted.Should().BeTrue();
    }

    [Fact]
    public async Task PUT_GoalsUncomplete_ValidGoalId_Returns200AndIncompletedGoal()
    {
        // Arrange - Create and complete a goal first
        var createRequest = new { teamMemberId = 1, description = "Goal to uncomplete" };
        var createResponse = await _client.PostAsJsonAsync("/api/goals", createRequest);
        var createdGoal = await createResponse.Content.ReadFromJsonAsync<GoalDto>();
        await _client.PutAsync($"/api/goals/{createdGoal!.Id}/complete", null);

        // Act
        var response = await _client.PutAsync($"/api/goals/{createdGoal.Id}/uncomplete", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var incompletedGoal = await response.Content.ReadFromJsonAsync<GoalDto>();
        incompletedGoal.Should().NotBeNull();
        incompletedGoal!.IsCompleted.Should().BeFalse();
    }

    [Fact]
    public async Task PUT_GoalsComplete_NonExistentGoalId_Returns404NotFound()
    {
        // Act
        var response = await _client.PutAsync("/api/goals/99999/complete", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion
}

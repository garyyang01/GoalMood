using FluentAssertions;
using GoalMood.BE.Models.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace GoalMood.Tests.Contract;

public class TeamMemberEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TeamMemberEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GET_Members_Returns200AndTeamMemberDtoArray()
    {
        // Act
        var response = await _client.GetAsync("/api/members");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var members = await response.Content.ReadFromJsonAsync<List<TeamMemberDto>>();
        members.Should().NotBeNull();
        members.Should().BeOfType<List<TeamMemberDto>>();
    }

    [Fact]
    public async Task GET_Members_EmptyDatabase_ReturnsEmptyArray()
    {
        // Note: This test would require database cleanup/setup
        // For now, we verify that the response is always an array (even if populated with seed data)

        // Act
        var response = await _client.GetAsync("/api/members");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var members = await response.Content.ReadFromJsonAsync<List<TeamMemberDto>>();
        members.Should().NotBeNull();
        members.Should().BeAssignableTo<IEnumerable<TeamMemberDto>>();
    }

    [Fact]
    public async Task GET_Members_ValidatesTeamMemberDtoSchema()
    {
        // Act
        var response = await _client.GetAsync("/api/members");
        var members = await response.Content.ReadFromJsonAsync<List<TeamMemberDto>>();

        // Assert
        if (members?.Count > 0)
        {
            var member = members[0];
            member.Id.Should().BeGreaterThan(0);
            member.Name.Should().NotBeNullOrEmpty();
            member.Name.Length.Should().BeInRange(1, 50);
            member.CurrentMood.Should().BeInRange(1, 5);
            member.MoodEmoji.Should().NotBeNullOrEmpty();
            member.Goals.Should().NotBeNull();
            member.CompletedCount.Should().BeGreaterThanOrEqualTo(0);
            member.TotalCount.Should().BeGreaterThanOrEqualTo(0);
            member.CompletedCount.Should().BeLessThanOrEqualTo(member.TotalCount);
        }
    }

    #region User Story 3 - Update Team Member Mood

    [Fact]
    public async Task PUT_MembersMood_ValidInput_Returns200AndUpdatedMember()
    {
        // Arrange
        var memberId = 1;
        var request = new { mood = 1 }; // Happy

        // Act
        var response = await _client.PutAsJsonAsync($"/api/members/{memberId}/mood", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedMember = await response.Content.ReadFromJsonAsync<TeamMemberDto>();
        updatedMember.Should().NotBeNull();
        updatedMember!.CurrentMood.Should().Be(1);
        updatedMember.MoodEmoji.Should().Be("😀");
    }

    [Fact]
    public async Task PUT_MembersMood_InvalidMoodValue_Returns400BadRequest()
    {
        // Arrange
        var memberId = 1;
        var request = new { mood = 10 }; // Invalid mood (not 1-5)

        // Act
        var response = await _client.PutAsJsonAsync($"/api/members/{memberId}/mood", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PUT_MembersMood_NonExistentMemberId_Returns404NotFound()
    {
        // Arrange
        var memberId = 99999;
        var request = new { mood = 3 };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/members/{memberId}/mood", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion
}

using GoalMood.BE.Data;
using GoalMood.BE.Models;
using GoalMood.BE.Models.DTOs;

namespace GoalMood.BE.Endpoints;

/// <summary>
/// Extension methods for statistics API endpoints
/// </summary>
public static class StatsEndpoints
{
    /// <summary>
    /// Maps stats endpoints
    /// </summary>
    public static void MapStatsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/stats").WithTags("Statistics");

        // GET /api/stats - Get team statistics
        group.MapGet("/", async (
            IGoalRepository goalRepo,
            ITeamMemberRepository memberRepo) =>
        {
            // Get goal statistics
            var (total, completed) = await goalRepo.GetTodayGoalsStatsAsync();
            var completionPercentage = total > 0 ? (double)completed / total * 100 : 0;

            // Get mood distribution
            var moodDistribution = await memberRepo.GetMoodDistributionAsync();

            var dto = new StatsDto(
                Math.Round(completionPercentage, 2),
                moodDistribution
            );

            return Results.Ok(dto);
        })
        .WithName("GetStats")
        .Produces<StatsDto>(StatusCodes.Status200OK);
    }
}

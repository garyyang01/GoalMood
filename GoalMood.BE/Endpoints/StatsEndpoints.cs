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
            ITeamMemberRepository memberRepo,
            ILogger<Program> logger) =>
        {
            logger.LogInformation("Fetching team statistics");

            // Get goal statistics
            var (total, completed) = await goalRepo.GetTodayGoalsStatsAsync();
            var completionPercentage = total > 0 ? (double)completed / total * 100 : 0;

            logger.LogInformation("Goal stats: {Completed}/{Total} goals completed ({Percentage}%)",
                completed, total, Math.Round(completionPercentage, 2));

            // Get mood distribution
            var moodDistribution = await memberRepo.GetMoodDistributionAsync();

            logger.LogInformation("Mood distribution retrieved: {Count} mood entries", moodDistribution.Count);

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

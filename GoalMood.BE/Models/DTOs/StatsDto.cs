namespace GoalMood.BE.Models.DTOs;

/// <summary>
/// Data transfer object for team statistics
/// </summary>
public record StatsDto(
    double CompletionPercentage,
    Dictionary<Mood, int> MoodDistribution
);

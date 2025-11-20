using GoalMood.BE.Models;

namespace GoalMood.BE.Data;

/// <summary>
/// Repository interface for goal data access
/// </summary>
public interface IGoalRepository
{
    /// <summary>
    /// Creates a new goal
    /// </summary>
    Task<Goal> CreateAsync(Goal goal);

    /// <summary>
    /// Deletes a goal by ID
    /// </summary>
    Task<bool> DeleteAsync(int goalId);

    /// <summary>
    /// Updates a goal's completion status
    /// </summary>
    Task<bool> UpdateCompletionAsync(int goalId, bool isCompleted);

    /// <summary>
    /// Gets today's goal statistics (for stats endpoint)
    /// </summary>
    Task<(int Total, int Completed)> GetTodayGoalsStatsAsync();
}

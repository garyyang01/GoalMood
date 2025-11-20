using GoalMood.BE.Models;

namespace GoalMood.BE.Data;

/// <summary>
/// Repository interface for team member data access
/// </summary>
public interface ITeamMemberRepository
{
    /// <summary>
    /// Gets all team members with their today's goals
    /// </summary>
    Task<IEnumerable<TeamMember>> GetAllWithTodayGoalsAsync();

    /// <summary>
    /// Updates a team member's mood
    /// </summary>
    Task<bool> UpdateMoodAsync(int memberId, Mood mood);

    /// <summary>
    /// Gets mood distribution across all team members
    /// </summary>
    Task<Dictionary<Mood, int>> GetMoodDistributionAsync();
}

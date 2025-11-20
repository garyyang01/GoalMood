namespace GoalMood.BE.Models;

/// <summary>
/// Represents a team member
/// </summary>
public class TeamMember
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Member name (1-50 characters)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Current mood state
    /// </summary>
    public Mood CurrentMood { get; set; } = Mood.Neutral;

    /// <summary>
    /// Navigation property for goals (not mapped to DB in Dapper)
    /// </summary>
    public List<Goal>? Goals { get; set; }
}

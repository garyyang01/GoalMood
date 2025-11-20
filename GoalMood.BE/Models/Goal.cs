namespace GoalMood.BE.Models;

/// <summary>
/// Represents a daily goal assigned to a team member
/// </summary>
public class Goal
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// ID of the team member who owns this goal
    /// </summary>
    public int TeamMemberId { get; set; }

    /// <summary>
    /// Goal description (1-500 characters)
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Whether the goal has been completed
    /// </summary>
    public bool IsCompleted { get; set; } = false;

    /// <summary>
    /// Date when the goal was created (defaults to today)
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    /// <summary>
    /// Navigation property for team member (not mapped to DB in Dapper)
    /// </summary>
    public TeamMember? TeamMember { get; set; }
}

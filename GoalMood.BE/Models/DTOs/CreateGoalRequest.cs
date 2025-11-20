namespace GoalMood.BE.Models.DTOs;

/// <summary>
/// Request model for creating a new goal
/// </summary>
public record CreateGoalRequest(
    int TeamMemberId,
    string Description
);

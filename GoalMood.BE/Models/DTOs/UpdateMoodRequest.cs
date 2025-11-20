namespace GoalMood.BE.Models.DTOs;

/// <summary>
/// Request model for updating a team member's mood
/// </summary>
public record UpdateMoodRequest(
    Mood Mood
);

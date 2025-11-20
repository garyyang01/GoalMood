namespace GoalMood.BE.Models.DTOs;

/// <summary>
/// Data transfer object for goal data in API responses
/// </summary>
public record GoalDto(
    int Id,
    int TeamMemberId,
    string Description,
    bool IsCompleted,
    DateTime CreatedDate
);

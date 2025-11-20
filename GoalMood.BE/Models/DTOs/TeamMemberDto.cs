namespace GoalMood.BE.Models.DTOs;

/// <summary>
/// Data transfer object for team member data in API responses (includes goals)
/// </summary>
public record TeamMemberDto(
    int Id,
    string Name,
    Mood CurrentMood,
    string MoodEmoji,
    List<GoalDto> Goals,
    int CompletedCount,
    int TotalCount
);

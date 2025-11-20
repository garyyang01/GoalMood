namespace GoalMood.BE.Models;

/// <summary>
/// Represents the mood state of a team member
/// </summary>
public enum Mood
{
    /// <summary>
    /// 😀 Happy
    /// </summary>
    Happy = 1,

    /// <summary>
    /// 😊 Content
    /// </summary>
    Content = 2,

    /// <summary>
    /// 😐 Neutral
    /// </summary>
    Neutral = 3,

    /// <summary>
    /// 😞 Sad
    /// </summary>
    Sad = 4,

    /// <summary>
    /// 😤 Stressed
    /// </summary>
    Stressed = 5
}

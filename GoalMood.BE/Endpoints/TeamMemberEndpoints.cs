using GoalMood.BE.Data;
using GoalMood.BE.Models;
using GoalMood.BE.Models.DTOs;

namespace GoalMood.BE.Endpoints;

/// <summary>
/// Extension methods for team member API endpoints
/// </summary>
public static class TeamMemberEndpoints
{
    /// <summary>
    /// Maps team member endpoints
    /// </summary>
    public static void MapTeamMemberEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/members").WithTags("Team Members");

        // GET /api/members - Get all team members with their today's goals
        group.MapGet("/", async (ITeamMemberRepository repo) =>
        {
            var members = await repo.GetAllWithTodayGoalsAsync();
            var dtos = members.Select(m => new TeamMemberDto(
                m.Id,
                m.Name,
                m.CurrentMood,
                GetMoodEmoji(m.CurrentMood),
                m.Goals?.Select(g => new GoalDto(
                    g.Id,
                    g.TeamMemberId,
                    g.Description,
                    g.IsCompleted,
                    g.CreatedDate
                )).ToList() ?? new List<GoalDto>(),
                m.Goals?.Count(g => g.IsCompleted) ?? 0,
                m.Goals?.Count ?? 0
            ));

            return Results.Ok(dtos);
        })
        .WithName("GetAllMembers")
        .Produces<IEnumerable<TeamMemberDto>>(StatusCodes.Status200OK);

        // PUT /api/members/{memberId}/mood - Update a member's mood
        group.MapPut("/{memberId}/mood", async (
            int memberId,
            UpdateMoodRequest request,
            ITeamMemberRepository repo) =>
        {
            // Validate mood value
            if (!Enum.IsDefined(typeof(Mood), request.Mood))
            {
                return Results.BadRequest(new { error = "Mood must be between 1-5" });
            }

            var success = await repo.UpdateMoodAsync(memberId, request.Mood);
            if (!success)
            {
                return Results.NotFound(new { error = "Team member not found" });
            }

            // Return updated member
            var members = await repo.GetAllWithTodayGoalsAsync();
            var member = members.FirstOrDefault(m => m.Id == memberId);

            if (member == null)
            {
                return Results.NotFound();
            }

            var dto = new TeamMemberDto(
                member.Id,
                member.Name,
                member.CurrentMood,
                GetMoodEmoji(member.CurrentMood),
                member.Goals?.Select(g => new GoalDto(
                    g.Id,
                    g.TeamMemberId,
                    g.Description,
                    g.IsCompleted,
                    g.CreatedDate
                )).ToList() ?? new List<GoalDto>(),
                member.Goals?.Count(g => g.IsCompleted) ?? 0,
                member.Goals?.Count ?? 0
            );

            return Results.Ok(dto);
        })
        .WithName("UpdateMemberMood")
        .Produces<TeamMemberDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);
    }

    /// <summary>
    /// Converts Mood enum to emoji string
    /// </summary>
    private static string GetMoodEmoji(Mood mood)
    {
        return mood switch
        {
            Mood.Happy => "😀",
            Mood.Content => "😊",
            Mood.Neutral => "😐",
            Mood.Sad => "😞",
            Mood.Stressed => "😤",
            _ => "😐"
        };
    }
}

using System.Data;
using Dapper;
using GoalMood.BE.Models;

namespace GoalMood.BE.Data;

/// <summary>
/// Repository implementation for team member data access using Dapper
/// </summary>
public class TeamMemberRepository : ITeamMemberRepository
{
    private readonly IDbConnection _db;

    public TeamMemberRepository(IDbConnection db)
    {
        _db = db;
    }

    /// <summary>
    /// Gets all team members with their today's goals using Dapper multi-mapping
    /// Avoids N+1 queries by loading everything in one query
    /// </summary>
    public async Task<IEnumerable<TeamMember>> GetAllWithTodayGoalsAsync()
    {
        var sql = @"
            SELECT
                tm.Id, tm.Name, tm.CurrentMood,
                g.Id, g.TeamMemberId, g.Description, g.IsCompleted, g.CreatedDate
            FROM TeamMembers tm
            LEFT JOIN Goals g ON tm.Id = g.TeamMemberId
                AND DATE(g.CreatedDate) = DATE('now', 'localtime')
            ORDER BY tm.Id, g.Id";

        var memberDict = new Dictionary<int, TeamMember>();

        await _db.QueryAsync<TeamMember, Goal?, TeamMember>(
            sql,
            (member, goal) =>
            {
                if (!memberDict.TryGetValue(member.Id, out var existingMember))
                {
                    existingMember = member;
                    existingMember.Goals = new List<Goal>();
                    memberDict.Add(member.Id, existingMember);
                }

                if (goal != null)
                {
                    existingMember.Goals!.Add(goal);
                }

                return existingMember;
            },
            splitOn: "Id"
        );

        return memberDict.Values;
    }

    /// <summary>
    /// Updates a team member's mood
    /// </summary>
    public async Task<bool> UpdateMoodAsync(int memberId, Mood mood)
    {
        var sql = @"
            UPDATE TeamMembers
            SET CurrentMood = @Mood
            WHERE Id = @MemberId";

        var affected = await _db.ExecuteAsync(sql, new { MemberId = memberId, Mood = mood });
        return affected > 0;
    }

    /// <summary>
    /// Gets mood distribution across all team members
    /// </summary>
    public async Task<Dictionary<Mood, int>> GetMoodDistributionAsync()
    {
        var sql = @"
            SELECT CurrentMood, COUNT(*) as Count
            FROM TeamMembers
            GROUP BY CurrentMood";

        var results = await _db.QueryAsync<(Mood Mood, int Count)>(sql);

        return results.ToDictionary(r => r.Mood, r => r.Count);
    }
}

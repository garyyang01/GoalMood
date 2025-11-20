using System.Data;
using Dapper;
using GoalMood.BE.Models;

namespace GoalMood.BE.Data;

/// <summary>
/// Repository implementation for goal data access using Dapper
/// </summary>
public class GoalRepository : IGoalRepository
{
    private readonly IDbConnection _db;

    public GoalRepository(IDbConnection db)
    {
        _db = db;
    }

    /// <summary>
    /// Creates a new goal with parameterized query to prevent SQL injection
    /// </summary>
    public async Task<Goal> CreateAsync(Goal goal)
    {
        var sql = @"
            INSERT INTO Goals (TeamMemberId, Description, IsCompleted, CreatedDate)
            VALUES (@TeamMemberId, @Description, @IsCompleted, @CreatedDate);
            SELECT last_insert_rowid();";

        var id = await _db.ExecuteScalarAsync<int>(sql, goal);
        goal.Id = id;
        return goal;
    }

    /// <summary>
    /// Deletes a goal by ID
    /// </summary>
    public async Task<bool> DeleteAsync(int goalId)
    {
        var sql = "DELETE FROM Goals WHERE Id = @GoalId";
        var affected = await _db.ExecuteAsync(sql, new { GoalId = goalId });
        return affected > 0;
    }

    /// <summary>
    /// Updates a goal's completion status
    /// </summary>
    public async Task<bool> UpdateCompletionAsync(int goalId, bool isCompleted)
    {
        var sql = @"
            UPDATE Goals
            SET IsCompleted = @IsCompleted
            WHERE Id = @GoalId";

        var affected = await _db.ExecuteAsync(sql, new { GoalId = goalId, IsCompleted = isCompleted });
        return affected > 0;
    }

    /// <summary>
    /// Gets today's goal statistics
    /// </summary>
    public async Task<(int Total, int Completed)> GetTodayGoalsStatsAsync()
    {
        var sql = @"
            SELECT
                COUNT(*) as Total,
                SUM(CASE WHEN IsCompleted = 1 THEN 1 ELSE 0 END) as Completed
            FROM Goals
            WHERE DATE(CreatedDate) = DATE('now', 'localtime')";

        var result = await _db.QuerySingleOrDefaultAsync<(int Total, int Completed)>(sql);
        return result;
    }
}

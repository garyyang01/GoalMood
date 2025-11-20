using GoalMood.BE.Data;
using GoalMood.BE.Models;
using GoalMood.BE.Models.DTOs;

namespace GoalMood.BE.Endpoints;

/// <summary>
/// Extension methods for goal API endpoints
/// </summary>
public static class GoalEndpoints
{
    /// <summary>
    /// Maps goal endpoints
    /// </summary>
    public static void MapGoalEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/goals").WithTags("Goals");

        // POST /api/goals - Create a new goal
        group.MapPost("/", async (
            CreateGoalRequest request,
            IGoalRepository goalRepo,
            ITeamMemberRepository memberRepo) =>
        {
            // Validate request
            if (request.TeamMemberId <= 0)
            {
                return Results.BadRequest(new { error = "Team member ID is required" });
            }

            if (string.IsNullOrWhiteSpace(request.Description))
            {
                return Results.BadRequest(new { error = "Description is required" });
            }

            if (request.Description.Length > 500)
            {
                return Results.BadRequest(new { error = "Description cannot exceed 500 characters" });
            }

            // Check if team member exists
            var members = await memberRepo.GetAllWithTodayGoalsAsync();
            if (!members.Any(m => m.Id == request.TeamMemberId))
            {
                return Results.NotFound(new { error = "Team member not found" });
            }

            // Create goal
            var goal = new Goal
            {
                TeamMemberId = request.TeamMemberId,
                Description = request.Description,
                IsCompleted = false,
                CreatedDate = DateTime.Now
            };

            goal = await goalRepo.CreateAsync(goal);

            var dto = new GoalDto(
                goal.Id,
                goal.TeamMemberId,
                goal.Description,
                goal.IsCompleted,
                goal.CreatedDate
            );

            return Results.Created($"/api/goals/{goal.Id}", dto);
        })
        .WithName("CreateGoal")
        .Produces<GoalDto>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);

        // DELETE /api/goals/{goalId} - Delete a goal
        group.MapDelete("/{goalId}", async (int goalId, IGoalRepository repo) =>
        {
            var success = await repo.DeleteAsync(goalId);
            if (!success)
            {
                return Results.NotFound(new { error = "Goal not found" });
            }

            return Results.NoContent();
        })
        .WithName("DeleteGoal")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);

        // PUT /api/goals/{goalId}/complete - Mark goal as complete
        group.MapPut("/{goalId}/complete", async (int goalId, IGoalRepository repo) =>
        {
            var success = await repo.UpdateCompletionAsync(goalId, true);
            if (!success)
            {
                return Results.NotFound(new { error = "Goal not found" });
            }

            return Results.Ok(new { message = "Goal marked as complete" });
        })
        .WithName("CompleteGoal")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // PUT /api/goals/{goalId}/uncomplete - Mark goal as incomplete
        group.MapPut("/{goalId}/uncomplete", async (int goalId, IGoalRepository repo) =>
        {
            var success = await repo.UpdateCompletionAsync(goalId, false);
            if (!success)
            {
                return Results.NotFound(new { error = "Goal not found" });
            }

            return Results.Ok(new { message = "Goal marked as incomplete" });
        })
        .WithName("UncompleteGoal")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
}

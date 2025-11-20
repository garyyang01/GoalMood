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
            ITeamMemberRepository memberRepo,
            ILogger<Program> logger) =>
        {
            logger.LogInformation("Creating goal for member {MemberId}: {Description}", request.TeamMemberId, request.Description);

            // Validate request
            if (request.TeamMemberId <= 0)
            {
                logger.LogWarning("Invalid team member ID: {MemberId}", request.TeamMemberId);
                return Results.BadRequest(new { error = "Team member ID is required" });
            }

            if (string.IsNullOrWhiteSpace(request.Description))
            {
                logger.LogWarning("Empty goal description for member {MemberId}", request.TeamMemberId);
                return Results.BadRequest(new { error = "Description is required" });
            }

            if (request.Description.Length > 500)
            {
                logger.LogWarning("Goal description too long ({Length} chars) for member {MemberId}", request.Description.Length, request.TeamMemberId);
                return Results.BadRequest(new { error = "Description cannot exceed 500 characters" });
            }

            // Check if team member exists
            var members = await memberRepo.GetAllWithTodayGoalsAsync();
            if (!members.Any(m => m.Id == request.TeamMemberId))
            {
                logger.LogWarning("Team member {MemberId} not found when creating goal", request.TeamMemberId);
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
            logger.LogInformation("Successfully created goal {GoalId} for member {MemberId}", goal.Id, request.TeamMemberId);

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
        group.MapDelete("/{goalId}", async (int goalId, IGoalRepository repo, ILogger<Program> logger) =>
        {
            logger.LogInformation("Deleting goal {GoalId}", goalId);
            var success = await repo.DeleteAsync(goalId);
            if (!success)
            {
                logger.LogWarning("Goal {GoalId} not found for deletion", goalId);
                return Results.NotFound(new { error = "Goal not found" });
            }

            logger.LogInformation("Successfully deleted goal {GoalId}", goalId);
            return Results.NoContent();
        })
        .WithName("DeleteGoal")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);

        // PUT /api/goals/{goalId}/complete - Mark goal as complete
        group.MapPut("/{goalId}/complete", async (int goalId, IGoalRepository repo, ILogger<Program> logger) =>
        {
            logger.LogInformation("Marking goal {GoalId} as complete", goalId);
            var success = await repo.UpdateCompletionAsync(goalId, true);
            if (!success)
            {
                logger.LogWarning("Goal {GoalId} not found for completion", goalId);
                return Results.NotFound(new { error = "Goal not found" });
            }

            logger.LogInformation("Successfully marked goal {GoalId} as complete", goalId);
            return Results.Ok(new { message = "Goal marked as complete" });
        })
        .WithName("CompleteGoal")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // PUT /api/goals/{goalId}/uncomplete - Mark goal as incomplete
        group.MapPut("/{goalId}/uncomplete", async (int goalId, IGoalRepository repo, ILogger<Program> logger) =>
        {
            logger.LogInformation("Marking goal {GoalId} as incomplete", goalId);
            var success = await repo.UpdateCompletionAsync(goalId, false);
            if (!success)
            {
                logger.LogWarning("Goal {GoalId} not found for uncompletion", goalId);
                return Results.NotFound(new { error = "Goal not found" });
            }

            logger.LogInformation("Successfully marked goal {GoalId} as incomplete", goalId);
            return Results.Ok(new { message = "Goal marked as incomplete" });
        })
        .WithName("UncompleteGoal")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
}

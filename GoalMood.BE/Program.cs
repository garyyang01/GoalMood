using System.Data;
using Microsoft.Data.Sqlite;
using Dapper;
using GoalMood.BE.Data;
using GoalMood.BE.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Configure connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=GoalMood.db;Mode=ReadWriteCreate;Cache=Shared";

// Register IDbConnection with SQLite
builder.Services.AddScoped<IDbConnection>(sp =>
{
    var connection = new SqliteConnection(connectionString);
    connection.Open();

    // Enable WAL mode for better concurrency (Decision D2 from research.md)
    using var command = connection.CreateCommand();
    command.CommandText = "PRAGMA journal_mode=WAL;";
    command.ExecuteNonQuery();

    return connection;
});

// Register repositories
builder.Services.AddScoped<ITeamMemberRepository, TeamMemberRepository>();
builder.Services.AddScoped<IGoalRepository, GoalRepository>();

// Configure CORS to allow frontend (multiple Vite ports for development)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:5174", "http://localhost:5175")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Run database migrations on startup
RunMigrations(connectionString);

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "GoalMood API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

// Global error handling middleware
app.Use(async (context, next) =>
{
    try
    {
        await next(context);
    }
    catch (Exception ex)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Unhandled exception occurred");

        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { error = "Internal server error" });
    }
});

app.UseCors("AllowFrontend");

// Map endpoints
app.MapTeamMemberEndpoints();
app.MapGoalEndpoints();
app.MapStatsEndpoints();

app.Run();

/// <summary>
/// Runs database migrations on application startup
/// </summary>
void RunMigrations(string connString)
{
    using var connection = new SqliteConnection(connString);
    connection.Open();

    // Run initial schema migration
    var schemaScript = File.ReadAllText("Migrations/001_InitialSchema.sql");
    connection.Execute(schemaScript);

    // Run seed data if needed (check if data already exists)
    var memberCount = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM TeamMembers");
    if (memberCount == 0)
    {
        var seedScript = File.ReadAllText("Migrations/seed-data.sql");
        connection.Execute(seedScript);
    }
}

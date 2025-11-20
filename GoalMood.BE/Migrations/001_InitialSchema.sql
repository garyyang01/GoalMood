-- Enable foreign key constraints
PRAGMA foreign_keys = ON;

-- Enable WAL mode for better concurrency
PRAGMA journal_mode = WAL;

-- Create TeamMembers table
CREATE TABLE IF NOT EXISTS TeamMembers (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL CHECK(LENGTH(Name) > 0 AND LENGTH(Name) <= 50),
    CurrentMood INTEGER NOT NULL DEFAULT 3 CHECK(CurrentMood BETWEEN 1 AND 5)
);

-- Create Goals table
CREATE TABLE IF NOT EXISTS Goals (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    TeamMemberId INTEGER NOT NULL,
    Description TEXT NOT NULL CHECK(LENGTH(Description) > 0 AND LENGTH(Description) <= 500),
    IsCompleted INTEGER NOT NULL DEFAULT 0 CHECK(IsCompleted IN (0, 1)),
    CreatedDate TEXT NOT NULL DEFAULT (datetime('now', 'localtime')),
    FOREIGN KEY (TeamMemberId) REFERENCES TeamMembers(Id) ON DELETE CASCADE
);

-- Create indexes for performance
CREATE INDEX IF NOT EXISTS idx_goals_created_date ON Goals(CreatedDate);
CREATE INDEX IF NOT EXISTS idx_goals_team_member ON Goals(TeamMemberId);

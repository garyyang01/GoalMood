-- Seed test data for development environment
-- 5 team members with different moods, 6 goals

-- Insert test team members
INSERT INTO TeamMembers (Name, CurrentMood) VALUES
    ('Alice Chen', 1),     -- Happy
    ('Bob Wang', 3),       -- Neutral
    ('Carol Lin', 2),      -- Content
    ('David Wu', 4),       -- Sad
    ('Eve Huang', 5);      -- Stressed

-- Insert test goals (today's date)
INSERT INTO Goals (TeamMemberId, Description, IsCompleted, CreatedDate) VALUES
    (1, 'Complete requirement analysis document', 1, datetime('now', 'localtime')),
    (1, 'Meet with client to confirm specifications', 0, datetime('now', 'localtime')),
    (2, 'Perform code review', 0, datetime('now', 'localtime')),
    (3, 'Write unit tests', 1, datetime('now', 'localtime')),
    (3, 'Fix Bug #123', 1, datetime('now', 'localtime')),
    (4, 'Learn new technology framework', 0, datetime('now', 'localtime'));

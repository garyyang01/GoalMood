# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**GoalMood** is a minimal full-stack team goal tracking and mood monitoring application built as a learning exercise for spec-driven development with AI pairing. The project demonstrates requirement gathering, task breaking, and guided AI development using Spec Kit commands.

**Key Constraints:**
- **Project Duration**: 45 minutes end-to-end implementation target
- **Target Users**: Small teams for daily standups
- **Development Approach**: Spec-driven development, not guesswork

## Technology Stack

### Backend (GoalMood.BE)
- **.NET 8** Web API
- **Dapper ORM** for data access (NOT Entity Framework)
- **SQLite** (local file) OR SQL Server on Docker
- Minimal API pattern with Swagger/OpenAPI
- TypeScript strict mode equivalence via C# nullable reference types

### Frontend (Not Yet Implemented)
- **Vue 3** with TypeScript
- **Composition API** with Composables pattern
- **DaisyUI** (Tailwind CSS component library)
- TypeScript strict mode enabled

### Database
- **SQLite** recommended for zero-setup development
- Alternative: SQL Server in Docker for production-like environment

## Common Commands

### Backend Development

```bash
# Navigate to backend directory
cd GoalMood.BE

# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run the application (development mode)
dotnet run

# Run with watch mode (hot reload)
dotnet watch run

# Run tests (when implemented)
dotnet test

# Clean build artifacts
dotnet clean
```

The backend runs on `https://localhost:5001` (HTTPS) and `http://localhost:5000` (HTTP) by default.

### Frontend Development (When Implemented)

```bash
# Navigate to frontend directory
cd GoalMood.FE

# Install dependencies
npm install

# Run development server
npm run dev

# Build for production
npm run build

# Run linting
npm run lint

# Run type checking
npm run type-check
```

## Architecture

### Current State
- **GoalMood.BE/**: .NET 8 Web API project with minimal template (WeatherForecast example)
- **Documentation/**: Contains the technical review and PRD
- **.specify/**: Spec Kit templates for spec-driven development workflow
- **.claude/commands/**: Spec Kit slash commands for planning, specification, and task management

### Planned Architecture

**Backend Structure (to be implemented):**
- `/Controllers` or minimal API endpoints: RESTful endpoints for goals and mood
- `/Models`: Domain entities (TeamMember, Goal, Mood)
- `/Data`: Dapper repositories and database context
- `/Services`: Business logic layer (optional, keep minimal)
- `/Migrations`: SQL migration scripts for schema setup

**Frontend Structure (to be implemented):**
- `/src/components`: Vue components (MemberCard, GoalInput, MoodSelector, StatsPanel)
- `/src/composables`: Reusable composition functions for API calls and state
- `/src/types`: TypeScript interfaces matching backend models
- `/src/api`: API client layer for backend communication

### Data Model

**Core Entities:**

```csharp
// TeamMember
- Id (int, PK)
- Name (string)
- CurrentMood (enum: Happy, Content, Neutral, Sad, Stressed)

// Goal
- Id (int, PK)
- TeamMemberId (int, FK)
- Description (string)
- IsCompleted (bool)
- CreatedDate (DateTime, defaults to today)

// Mood (optional tracking table)
- Id (int, PK)
- TeamMemberId (int, FK)
- MoodValue (enum)
- UpdatedAt (DateTime)
```

**Key API Endpoints (to be implemented):**
- `GET /api/members` - Get all team members with goals and current mood
- `POST /api/goals` - Add a new goal
- `PUT /api/goals/{id}/complete` - Mark goal as complete
- `DELETE /api/goals/{id}` - Delete a goal
- `PUT /api/members/{id}/mood` - Update member's current mood
- `GET /api/stats` - Get team statistics (completion %, mood distribution)

## Development Workflow

This project uses **Spec Kit** for structured development:

1. **Specify**: Use `/speckit.specify` to create or update feature specifications from natural language
2. **Plan**: Use `/speckit.plan` to generate implementation plan with design artifacts
3. **Tasks**: Use `/speckit.tasks` to generate dependency-ordered tasks from the plan
4. **Implement**: Use `/speckit.implement` to execute tasks systematically
5. **Analyze**: Use `/speckit.analyze` for cross-artifact consistency checks

### Key Principles

**What to Build (MVP ONLY):**
- Team member cards showing name, mood, goals, completion count
- Simple forms for adding goals and updating mood
- Basic stats panel (completion %, mood distribution)

**What NOT to Build:**
- ❌ User authentication/login
- ❌ Multi-day history or trends
- ❌ Email notifications
- ❌ Goal editing (only add/complete/delete)
- ❌ Responsive mobile design (desktop only)
- ❌ Dark mode, profile pages, admin controls

**Backend Patterns:**
- Use **Dapper** for all database operations, not Entity Framework
- Keep controllers thin, use service layer only if complexity requires it
- Prefer minimal API endpoints over traditional controllers for simplicity
- Use SQLite for local development (single file database in project root)

**Frontend Patterns (when implemented):**
- Use **Composition API** exclusively, not Options API
- Extract reusable logic into composables (e.g., `useGoals`, `useMoods`)
- Leverage **DaisyUI** components (btn, card, dropdown, checkbox, badge)
- Keep components focused: one responsibility per component

## Database Setup

### SQLite (Recommended)
```bash
# Database file will be created automatically at: ./GoalMood.db
# No setup required - just configure connection string in appsettings.json:
"ConnectionStrings": {
  "DefaultConnection": "Data Source=GoalMood.db"
}
```

### SQL Server (Alternative)
```bash
# Using Docker
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" \
  -p 1433:1433 --name goalmood-sql \
  -d mcr.microsoft.com/mssql/server:2022-latest

# Connection string in appsettings.json:
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost,1433;Database=GoalMood;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True"
}
```

## Code Style

### C# / .NET
- Use nullable reference types (already enabled in .csproj)
- Follow minimal API patterns for simple endpoints
- Use record types for DTOs and simple data structures
- Async/await for all I/O operations

### TypeScript / Vue (when implemented)
- Strict mode enabled
- Use `<script setup lang="ts">` syntax
- Define props with `defineProps<Props>()`
- Prefer `const` over `let`, avoid `var`
- Use Tailwind utility classes via DaisyUI components

## Project Context

This is a **learning-focused project** for practicing:
- Spec-driven development methodology
- AI pair programming with structured commands
- Breaking down requirements into actionable tasks
- Building full-stack applications with modern frameworks

The 45-minute target is aspirational - focus on quality implementation following the spec, not just speed.

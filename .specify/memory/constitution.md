# GoalMood Constitution

<!--
SYNC IMPACT REPORT - Version 1.0.0 (Initial Ratification)
==========================================================
Version Change: [none] → 1.0.0
Modified Principles: Initial creation
Added Sections: All sections (initial creation)
Removed Sections: None

Template Compatibility Status:
✅ plan-template.md - Compatible (constitution check section present)
✅ spec-template.md - Compatible (requirements and success criteria align)
✅ tasks-template.md - Compatible (test-driven and phase structure align)
✅ Command files - Compatible (all slash commands reference constitution principles)

Follow-up TODOs: None
==========================================================
-->

## Core Principles

### I. MVP-First Development

**Rule**: Every feature must deliver minimum viable value within the 45-minute implementation target. This is a learning project focused on spec-driven development, not production complexity.

**Non-Negotiable Requirements**:
- Build only what is explicitly specified in requirements
- Default to simplest possible implementation
- NO user authentication, multi-day history, email notifications, or responsive design
- NO goal editing (only add/complete/delete operations)
- Desktop-only interface (no mobile optimization)
- SQLite for database (no complex database setup)

**Rationale**: Rapid iteration and learning require ruthless scope control. Complexity without necessity defeats the educational purpose of spec-driven development with AI pairing.

### II. Technology Stack Enforcement

**Rule**: Use prescribed technologies exactly as specified. Deviations require explicit documentation and justification.

**Backend Stack (NON-NEGOTIABLE)**:
- .NET 8 Web API with minimal API pattern
- Dapper ORM for ALL database operations (Entity Framework is prohibited)
- SQLite for local development
- Nullable reference types enabled (TypeScript-equivalent strictness)
- Async/await for all I/O operations

**Frontend Stack (when implemented)**:
- Vue 3 with TypeScript strict mode
- Composition API exclusively (Options API prohibited)
- DaisyUI component library (Tailwind CSS based)
- Composables pattern for reusable logic (useGoals, useMoods)

**Rationale**: Technology constraints enable focused learning on spec-driven development patterns rather than framework evaluation. The stack is optimized for rapid prototyping with zero-setup requirements.

### III. Test-Driven Development (CONDITIONAL)

**Rule**: When tests are explicitly requested in requirements, they MUST be written before implementation. Tests are optional by default to meet the 45-minute target.

**When Tests Required**:
- User story acceptance scenarios explicitly demand tests
- Contract changes between frontend and backend
- Data model migrations that affect existing data
- Integration points with external systems

**Test Execution Flow**:
1. Write contract/integration tests first
2. Verify tests FAIL (red)
3. Implement minimum code to pass (green)
4. Refactor for clarity (refactor)
5. User validation before next cycle

**Test Organization**:
- `tests/contract/` - API contract tests (endpoint behavior)
- `tests/integration/` - End-to-end user journey tests
- `tests/unit/` - Optional, only if complexity justifies

**Rationale**: TDD ensures correctness but adds time overhead. Making tests conditional allows balancing quality with the 45-minute constraint while maintaining discipline when quality gates are critical.

### IV. Code Quality Standards

**Rule**: All code must meet production-quality standards despite the rapid timeline.

**C# Quality Requirements**:
- No nullable warnings (treat warnings as errors)
- Use record types for DTOs and simple data structures
- Minimal API endpoints over traditional controllers
- Keep business logic in service layer only if needed (default to thin controllers)
- Proper async/await usage (no blocking calls)

**TypeScript/Vue Quality Requirements (when implemented)**:
- Strict mode enabled (no implicit any, strict null checks)
- `<script setup lang="ts">` syntax exclusively
- Props defined with `defineProps<Props>()`
- Prefer `const` over `let`, never use `var`
- One component responsibility per file

**Code Organization**:
- Backend: `/Models`, `/Data` (Dapper repos), `/Services` (optional), minimal API endpoints
- Frontend: `/components`, `/composables`, `/types`, `/api`
- No orphaned code: delete unused code completely (no commenting out)

**Rationale**: Rapid development cannot compromise correctness. Nullable reference types and strict TypeScript prevent entire classes of runtime errors. Clean code patterns make AI-assisted refactoring safer.

### V. User Experience Consistency

**Rule**: UI/UX must follow DaisyUI component patterns with consistent interaction models.

**Component Usage Standards**:
- Use DaisyUI semantic classes: `btn`, `card`, `dropdown`, `checkbox`, `badge`
- No custom Tailwind overrides without documentation
- Consistent mood representation: emoji + text label
- Consistent goal states: pending (checkbox), completed (checkmark + strikethrough)

**Interaction Patterns**:
- All form submissions provide immediate visual feedback
- Error states show inline validation messages
- Loading states use DaisyUI loading spinner
- Success actions show brief confirmation (e.g., "Goal added!")

**Accessibility Baseline**:
- Semantic HTML (button elements for buttons, not divs)
- Keyboard navigation support (tab order, enter to submit)
- Screen reader labels for icon-only buttons
- Sufficient color contrast (DaisyUI default themes meet WCAG AA)

**Rationale**: DaisyUI provides consistent, accessible components out of the box. Adhering to its patterns ensures professional UX without design overhead.

### VI. Performance Requirements

**Rule**: Application must feel instant for the target scale (small teams, <20 members).

**Backend Performance Targets**:
- API responses: <100ms p95 for all endpoints (local SQLite)
- Database queries: Use Dapper for minimal ORM overhead
- No N+1 queries: eager load related data (goals with members)
- Connection pooling for database access

**Frontend Performance Targets**:
- Initial load: <2 seconds (dev server, no production build optimization required)
- UI interactions: <16ms response (60fps for mood updates, goal checks)
- State management: Vue reactivity only, no Vuex/Pinia overhead
- API calls: Debounce user input (e.g., goal descriptions)

**Scale Limits (Out of Scope)**:
- Max 20 team members (no pagination required)
- Max 100 goals total across team (no virtualization)
- Single-day view only (no historical data queries)

**Rationale**: Small team scope allows aggressive simplification. SQLite performance is excellent for <1000 rows. Vue reactivity handles <100 items without optimization.

### VII. Dapper-First Data Access

**Rule**: ALL database operations MUST use Dapper. Entity Framework is explicitly prohibited.

**Dapper Patterns**:
- Raw SQL queries in repository classes
- Parameterized queries for all user input (SQL injection prevention)
- Use `QueryAsync<T>` for reads, `ExecuteAsync` for writes
- Map to domain models (POCOs), not EF entities
- Transactions for multi-table operations (goal + mood updates)

**Migration Strategy**:
- SQL scripts in `/Migrations` directory
- Manual execution during development (no EF migration framework)
- Schema versioning in comments (e.g., `-- v1.0.0 initial schema`)

**Prohibited Patterns**:
- Entity Framework DbContext
- LINQ to Entities
- Auto-generated migrations
- Change tracking

**Rationale**: Dapper enforces understanding of SQL and data access patterns. It eliminates EF complexity and performance overhead while teaching database fundamentals.

## Data Model Constraints

**Core Entities** (NON-NEGOTIABLE):

```csharp
// TeamMember
- Id (int, PK, auto-increment)
- Name (string, max 100 chars, required)
- CurrentMood (enum: Happy, Content, Neutral, Sad, Stressed)

// Goal
- Id (int, PK, auto-increment)
- TeamMemberId (int, FK → TeamMember.Id)
- Description (string, max 500 chars, required)
- IsCompleted (bool, default false)
- CreatedDate (DateTime, default CURRENT_TIMESTAMP)
```

**Relationship Rules**:
- One-to-many: TeamMember → Goals (cascade delete)
- No many-to-many relationships (keep simple)
- No audit tables (no created_by, updated_by tracking)

**Schema Evolution**:
- Additive changes only during feature development
- Breaking changes require new migration script
- No ALTER TABLE during active development sessions

## API Contract Standards

**Endpoint Naming** (RESTful):
- `GET /api/members` - List all members with embedded goals and mood
- `POST /api/goals` - Create goal (body: { teamMemberId, description })
- `PUT /api/goals/{id}/complete` - Mark complete (no body)
- `DELETE /api/goals/{id}` - Remove goal (cascade safe)
- `PUT /api/members/{id}/mood` - Update mood (body: { mood })
- `GET /api/stats` - Aggregate stats (completion %, mood distribution)

**Response Format**:
- Success: HTTP 200/201 with JSON body
- Validation error: HTTP 400 with { "error": "message", "field": "fieldName" }
- Not found: HTTP 404 with { "error": "Resource not found" }
- Server error: HTTP 500 with { "error": "Internal error" } (log details server-side)

**Contract Versioning**:
- No versioning required for MVP (breaking changes break the app)
- Document all contracts in `/specs/###-feature/contracts/` during planning

## Development Workflow

**Spec-Driven Process** (MANDATORY):

1. **Specify** (`/speckit.specify`): Natural language → feature spec with user stories
2. **Plan** (`/speckit.plan`): Spec → implementation plan with design artifacts
3. **Tasks** (`/speckit.tasks`): Plan → dependency-ordered task list
4. **Implement** (`/speckit.implement`): Execute tasks systematically
5. **Analyze** (`/speckit.analyze`): Cross-artifact consistency validation

**User Story Independence**:
- Each story must be independently testable
- Prioritize stories (P1, P2, P3) by user value
- Implement P1 first for MVP, then incrementally add P2, P3
- Each story delivers value without depending on later stories

**Git Workflow**:
- Feature branch per spec: `###-feature-name` (e.g., `001-goal-tracking`)
- Commit after each task or logical group
- PR title: `feat(###): Brief description` (e.g., `feat(001): Add goal tracking`)
- Squash commits before merge to main

## Governance

**Constitution Supremacy**: This constitution overrides all other practices, examples, or conventions. When conflicts arise, constitution rules take precedence.

**Amendment Process**:
1. Identify principle violation or gap
2. Document why current rule is insufficient
3. Propose amendment with rationale
4. Update constitution version (semantic versioning)
5. Propagate changes to templates (plan, spec, tasks)
6. Update `.claude/commands/*.md` if governance affects AI workflows

**Versioning Policy**:
- **MAJOR**: Backward-incompatible changes (e.g., removing TDD requirement)
- **MINOR**: New principle added (e.g., adding security section)
- **PATCH**: Clarifications, examples, typo fixes (no semantic change)

**Compliance Review**:
- All PRs must verify compliance with applicable principles
- Complexity violations require explicit justification in plan.md
- `/speckit.analyze` validates cross-artifact consistency with constitution

**Runtime Guidance**:
- Development workflow details in `CLAUDE.md`
- Technology-specific patterns in `CLAUDE.md` (Dapper, Vue Composition API)
- This constitution focuses on non-negotiable rules, CLAUDE.md provides implementation guidance

**Version**: 1.0.0 | **Ratified**: 2025-01-18 | **Last Amended**: 2025-01-18

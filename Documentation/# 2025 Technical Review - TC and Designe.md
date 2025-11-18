# 2025 Technical Review - TC and Designer

# Product Requirements Document

## Team Daily Goal Tracker with Mood Sync

**Project Duration**: 45 minutes (end-to-end implementation)

**Target Users**: Small teams for daily standups

**Success Metric**: Full-stack app deployable and functional

---

## Learning Focus

- ✅ **Requirement Gathering** - Clarify and refine business needs into actionable specs
- ✅ **Task Breaking** - Decompose features into small, sequenced tasks
- ✅ **AI Pairing** - Guide AI agents effectively using Spec Kit commands
- ✅ **Spec-Driven Development** - Build from specification, not guesswork

---

## 1. Problem Statement

Teams need a quick way to track daily goals and monitor team morale in one place. Currently, goals and mood are tracked separately or not at all, making it hard for managers to see who's struggling or needs support.

---

## 2. Product Overview

A minimal full-stack web application where:

- Team members can see all team goals for the day
- Anyone can log/update their mood
- Goals can be marked complete
- Dashboard shows team completion % and overall mood

---

## 3. CORE Features (MVP Only)

### 3.1 Frontend

**Dashboard Page**

- Display all team members in cards
- Each card shows:
    - Member name
    - Current mood emoji (😀 😊 😐 😞 😤)
    - List of goals for the day
    - Goal completion count (2/3)
    - Checkbox to mark goals complete

**Simple Input Forms**

- Add Goal: dropdown to select team member + text input for goal description
- Update Mood: dropdown to select team member + mood emoji selector

**Stats Panel**

- Team goal completion % (simple number)
- Team mood indicator (show count: X happy, Y neutral, Z stressed)

---

## 4. OUT of Scope (Do NOT Build)

- ❌ User authentication/login
- ❌ Multi-day goal history
- ❌ Detailed mood analytics or charts
- ❌ Email notifications
- ❌ Admin controls for team management
- ❌ Goal editing (only add/complete/delete)
- ❌ Mood history or trends
- ❌ Recurring goals
- ❌ Goal categories or tags
- ❌ Responsive mobile design (desktop only)
- ❌ Dark mode
- ❌ Profile pages

---

## 5. Technical Constraints

### Frontend

- **Vue 3** with TypeScript, DaisyUI (Tailwind CSS component library)
- Use Composition API, **Composables**
- TypeScript strict mode enabled
- Leverage DaisyUI components (btn, card, dropdown, checkbox, etc.)

### Backend

- **.NET 8** Web API with Dapper ORM
- Dapper for data access (no Entity Framework)

### Database

- **SQLite** (local file) OR SQL Server on Docker
- SQLite recommended for simplicity (zero setup)
- Alternative: SQL Server in Docker

### Deployment

- Local development only
- Frontend: `npm run dev` or build for static hosting
- Backend: `dotnet run` from project folder
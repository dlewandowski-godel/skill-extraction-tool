# US-5.1 — View Own Skill Profile

**Epic:** Epic 5 — Employee Skill Profile  
**Status:** `[ ] Not Started` <!-- change to [x] Done when complete -->

---

## User Story

> As a user, I want to view my extracted skills grouped by taxonomy category so I can see my full skill profile at a glance.

---

## Acceptance Criteria

- [ ] `GET /api/profile/me` returns the authenticated user's skill profile
- [ ] Response includes: `userId`, `fullName`, `department`, `skills[]` (each with `skillId`, `skillName`, `category`, `proficiencyLevel`, `isManualOverride`, `extractedAt`)
- [ ] Skills are grouped by `category` in the response and rendered as grouped MUI `Card` sections
- [ ] If no documents have been processed yet, the profile page shows an empty state with a CTA to upload documents
- [ ] The profile page is accessible at `/profile`
- [ ] Data is fetched via TanStack Query `useQuery({ queryKey: ['profile', 'me'], queryFn: ... })`
- [ ] Page shows a loading skeleton while data is loading (MUI `Skeleton` components)

---

## Technical Notes

- API query uses `IMediator.Send(new GetMyProfileQuery())` — a MediatR `IRequest<EmployeeProfileDto>`
- `GetMyProfileQueryHandler` fetches from `EmployeeSkillRepository` filtered by current user ID (from JWT claims)

---

## Unit Tests

**Backend (`GetMyProfileQueryHandlerTests`):**
- [ ] Returns grouped skill list with correct categories for a user with existing skills
- [ ] Returns an empty skills list for a user with no processed documents
- [ ] Only returns skills belonging to the requesting user (not other users')

**Frontend (`useMyProfileQuery.test.ts`):**
- [ ] Returns profile data on a successful API response
- [ ] Returns `isLoading: true` while the request is in flight
- [ ] Returns error state on API failure
- [ ] Query key is `['profile', 'me']`

---

## Links

- [BACKLOG.md](../../BACKLOG.md)
- **Next:** [US-5.2](US-5.2-proficiency-indicators.md)

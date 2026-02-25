# US-6.1 — Top 10 Skills Bar Chart

**Epic:** Epic 6 — Admin Dashboard & Analytics  
**Status:** `[ ] Not Started` <!-- change to [x] Done when complete -->

---

## User Story

> As an admin, I want a bar chart of the top 10 skills across all employees so I can see what the organization knows best.

---

## Acceptance Criteria

- [ ] `GET /api/admin/analytics/top-skills?limit=10` returns skill name + employee count, sorted descending
- [ ] Frontend renders a horizontal bar chart using `recharts` (or MUI Charts if available)
- [ ] Chart shows skill name on Y-axis and employee count on X-axis
- [ ] Chart has a clear title: "Top Skills Across All Employees"
- [ ] Hovering a bar shows a tooltip with exact count
- [ ] Chart is responsive and fits in a MUI `Card` on the admin dashboard
- [ ] Data is loaded via TanStack Query and shows a skeleton while loading

---

## Technical Notes

- Recommended chart library: `recharts` — lightweight, responsive, good React support
- API query uses `GetTopSkillsQuery(limit)` MediatR query handler
- SQL: `GROUP BY skill_id ORDER BY COUNT(employee_id) DESC LIMIT @limit`

---

## Unit Tests

**Backend (`GetTopSkillsQueryHandlerTests`):**
- [ ] Returns results sorted by employee count descending
- [ ] Respects the `limit` parameter (e.g., limit=10 returns at most 10 entries)
- [ ] Returns an empty list when no skills have been extracted yet

**Frontend (`useTopSkillsQuery.test.ts`):**
- [ ] Returns a sorted array of `{ skillName, count }` on a successful response
- [ ] `staleTime` is at least 2 minutes
- [ ] Query key includes `['analytics', 'top-skills']`

---

## Links

- [BACKLOG.md](../../BACKLOG.md)
- **Next:** [US-6.2](US-6.2-skills-per-department.md)

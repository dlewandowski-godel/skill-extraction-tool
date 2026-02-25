# US-6.5 — Proficiency Distribution Chart

**Epic:** Epic 6 — Admin Dashboard & Analytics  
**Status:** `[ ] Not Started` <!-- change to [x] Done when complete -->

---

## User Story

> As an admin, I want a pie/donut chart of proficiency level distribution across the organization so I can gauge overall skill maturity.

---

## Acceptance Criteria

- [ ] `GET /api/admin/analytics/proficiency-distribution` returns count per proficiency level across all employee skills
- [ ] Frontend renders a donut chart (recharts `PieChart` with `innerRadius`) 
- [ ] Segments: Beginner (grey), Intermediate (blue), Advanced (orange), Expert (green) — consistent with US-5.2 colors
- [ ] Chart center shows the total number of skill entries
- [ ] Legend shows each level with its count and percentage
- [ ] Chart title: "Proficiency Distribution"
- [ ] Clicking a segment filters the employee list to show only employees with that proficiency level (optional stretch goal)

---

## Technical Notes

- Return format: `{ level: string, count: number }[]`
- SQL: `GROUP BY proficiency_level`

---

## Unit Tests

**Backend (`GetProficiencyDistributionQueryHandlerTests`):**
- [ ] Returns one entry per proficiency level present in the data
- [ ] Counts are accurate across all employees
- [ ] Returns an empty list when no skills have been extracted

---

## Links

- [BACKLOG.md](../../BACKLOG.md)
- **Prev:** [US-6.4](US-6.4-upload-activity-chart.md) | **Next:** [US-6.6](US-6.6-dashboard-tanstack-query.md)

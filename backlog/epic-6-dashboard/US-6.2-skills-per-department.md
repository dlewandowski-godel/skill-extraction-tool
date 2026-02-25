# US-6.2 — Skills per Department Chart

**Epic:** Epic 6 — Admin Dashboard & Analytics  
**Status:** `[ ] Not Started` <!-- change to [x] Done when complete -->

---

## User Story

> As an admin, I want a chart showing skill distribution per department/team so I can compare capabilities across groups.

---

## Acceptance Criteria

- [ ] `GET /api/admin/analytics/skills-by-department` returns a list of departments, each with their top skills and counts
- [ ] Frontend renders a grouped or stacked bar chart (recharts `BarChart` with multiple `Bar` series)
- [ ] Chart shows top 5 skills per department (configurable)
- [ ] A department filter dropdown allows selecting a single department to drill down
- [ ] Chart has a clear title: "Skill Distribution by Department"
- [ ] Data is loaded via TanStack Query; department filter change triggers a refetch
- [ ] Chart is responsive and fits in a MUI `Card`

---

## Technical Notes

- API groups `EmployeeSkills` by `Users.DepartmentId` and then by `SkillId`
- Return format: `{ department: string, skills: { name: string, count: number }[] }[]`

---

## Unit Tests

**Backend (`GetSkillsByDepartmentQueryHandlerTests`):**
- [ ] Returns one entry per department, each containing a skills list
- [ ] Skills within each department are sorted by count descending
- [ ] Employees without a department are excluded from results

---

## Links

- [BACKLOG.md](../../BACKLOG.md)
- **Prev:** [US-6.1](US-6.1-top-skills-chart.md) | **Next:** [US-6.3](US-6.3-skill-gap-analysis.md)

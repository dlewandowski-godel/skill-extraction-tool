# US-6.3 — Skill Gap Analysis View

**Epic:** Epic 6 — Admin Dashboard & Analytics  
**Status:** `[ ] Not Started` <!-- change to [x] Done when complete -->

---

## User Story

> As an admin, I want a skill gap analysis view (required skills per department vs. skills employees actually have) so I can identify training needs.

---

## Acceptance Criteria

- [ ] `GET /api/admin/analytics/skill-gaps` returns per-department: required skills, how many employees have them, and the gap (required - actual)
- [ ] Frontend renders this as a comparison bar chart or a table with visual indicators (green = covered, red = gap)
- [ ] A department selector filters the view to a specific department
- [ ] Required skills per department are configurable via the taxonomy admin (US-8.5)
- [ ] Gap is expressed as: `{ skill, required: true, employeesWithSkill: number, totalEmployees: number, gapPercent: number }`
- [ ] Skills with 0 employees having them are highlighted in red
- [ ] Chart/table title: "Skill Gap Analysis"
- [ ] Empty state shown if no required skills are configured for the selected department

---

## Technical Notes

- Depends on US-8.5 (required skills per department) being implemented first
- `gapPercent = 100 - (employeesWithSkill / totalEmployees * 100)`

---

## Unit Tests

**Backend (`GetSkillGapsQueryHandlerTests`):**
- [ ] Returns correct `gapPercent` when some but not all employees have a required skill
- [ ] Returns `gapPercent = 100` when no employees have a required skill
- [ ] Returns `gapPercent = 0` when all employees have the required skill
- [ ] Returns an empty list when no required skills are configured for a department

---

## Links

- [BACKLOG.md](../../BACKLOG.md)
- **Prev:** [US-6.2](US-6.2-skills-per-department.md) | **Next:** [US-6.4](US-6.4-upload-activity-chart.md)

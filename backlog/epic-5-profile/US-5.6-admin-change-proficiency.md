# US-5.6 — Admin: Change Proficiency Level

**Epic:** Epic 5 — Employee Skill Profile  
**Status:** `[ ] Not Started` <!-- change to [x] Done when complete -->

---

## User Story

> As an admin, I want to change the proficiency level of any skill on an employee's profile so the data remains accurate.

---

## Acceptance Criteria

- [ ] `PUT /api/admin/employees/{id}/skills/{skillId}` accepts `{ proficiencyLevel }` and updates the level
- [ ] After a manual update, `IsManualOverride` is set to `true` so re-extraction does not overwrite the value
- [ ] Valid proficiency levels: `Beginner`, `Intermediate`, `Advanced`, `Expert`
- [ ] Invalid level returns `400 Bad Request`
- [ ] Admin UI shows an inline editable proficiency chip or an edit button on each skill row
- [ ] Clicking edit opens a small popover/dialog with a proficiency level selector
- [ ] On success, the profile updates in place without a full page reload
- [ ] Action is logged: `[AuditLog] Admin {adminId} changed proficiency of skill {skillId} for employee {employeeId} to {level}`

---

## Technical Notes

- The `[AuditLog]` entries (US-5.4, 5.5, 5.6) should be structured logs captured by `ILogger` — no separate audit table needed for MVP

---

## Unit Tests

**Backend (`ChangeProficiencyCommandHandlerTests`):**
- [ ] Updates proficiency level and sets `IsManualOverride = true`
- [ ] Returns a `ValidationError` result for an invalid proficiency level string
- [ ] Returns a `NotFound` result when the `(employeeId, skillId)` pair does not exist

---

## Links

- [BACKLOG.md](../../BACKLOG.md)
- **Prev:** [US-5.5](US-5.5-admin-remove-skill.md)

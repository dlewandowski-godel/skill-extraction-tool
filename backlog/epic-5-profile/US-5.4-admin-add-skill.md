# US-5.4 — Admin: Manually Add Skill

**Epic:** Epic 5 — Employee Skill Profile  
**Status:** `[ ] Not Started` <!-- change to [x] Done when complete -->

---

## User Story

> As an admin, I want to manually add a skill to an employee's profile so I can correct or supplement extracted results.

---

## Acceptance Criteria

- [ ] `POST /api/admin/employees/{id}/skills` accepts `{ skillId, proficiencyLevel }` and adds the skill to the employee's profile
- [ ] Added skill has `IsManualOverride = true`
- [ ] If the employee already has the skill, the existing record is updated (upsert), not duplicated
- [ ] The skill must exist in the taxonomy; unknown `skillId` returns `400 Bad Request`
- [ ] Valid proficiency levels: `Beginner`, `Intermediate`, `Advanced`, `Expert`
- [ ] Admin UI shows an "Add Skill" button that opens a dialog with a searchable skill selector and proficiency dropdown
- [ ] On success, the profile view updates without a full page reload (TanStack Query cache invalidation)
- [ ] Action is logged: `[AuditLog] Admin {adminId} added skill {skillId} to employee {employeeId}`

---

## Technical Notes

- Use TanStack Query `useMutation` + `queryClient.invalidateQueries({ queryKey: ['employee-profile', id] })` on success
- Skill selector in dialog uses a MUI `Autocomplete` with taxonomy options loaded from `GET /api/skills` (cached)

---

## Unit Tests

**Backend (`AddSkillToEmployeeCommandHandlerTests`):**
- [ ] Adds the skill with `IsManualOverride = true` when the skill exists in the taxonomy
- [ ] Returns a `NotFound` result when the `skillId` does not exist in the taxonomy
- [ ] Upserts correctly: calling with a skill the employee already has updates it instead of inserting a duplicate

---

## Links

- [BACKLOG.md](../../BACKLOG.md)
- **Prev:** [US-5.3](US-5.3-admin-view-profile.md) | **Next:** [US-5.5](US-5.5-admin-remove-skill.md)

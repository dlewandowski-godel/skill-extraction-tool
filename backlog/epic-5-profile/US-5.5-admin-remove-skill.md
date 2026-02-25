# US-5.5 — Admin: Remove Skill

**Epic:** Epic 5 — Employee Skill Profile  
**Status:** `[ ] Not Started` <!-- change to [x] Done when complete -->

---

## User Story

> As an admin, I want to remove a skill from an employee's profile so incorrect extractions can be cleaned up.

---

## Acceptance Criteria

- [ ] `DELETE /api/admin/employees/{id}/skills/{skillId}` removes the skill from the employee's profile
- [ ] Endpoint is restricted to `Admin` role
- [ ] Non-existent `(employeeId, skillId)` pair returns `404 Not Found`
- [ ] A confirmation dialog is shown in the UI before the delete request is sent ("Remove 'Python' from John Doe's profile?")
- [ ] On success, the profile view updates without a full page reload (TanStack Query cache invalidation)
- [ ] Action is logged: `[AuditLog] Admin {adminId} removed skill {skillId} from employee {employeeId}`

---

## Technical Notes

- Soft delete is not required here — hard delete is fine for skill profile entries
- If the skill was auto-extracted from a document that is re-processed later, it will be re-added (this is expected behavior)

---

## Unit Tests

**Backend (`RemoveSkillFromEmployeeCommandHandlerTests`):**
- [ ] Successfully removes the skill when the `(employeeId, skillId)` pair exists
- [ ] Returns a `NotFound` result when the pair does not exist

---

## Links

- [BACKLOG.md](../../BACKLOG.md)
- **Prev:** [US-5.4](US-5.4-admin-add-skill.md) | **Next:** [US-5.6](US-5.6-admin-change-proficiency.md)

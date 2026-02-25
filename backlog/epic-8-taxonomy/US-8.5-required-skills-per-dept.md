# US-8.5 — Define Required Skills per Department

**Epic:** Epic 8 — Skill Taxonomy Management (Admin)  
**Status:** `[ ] Not Started` <!-- change to [x] Done when complete -->

---

## User Story

> As an admin, I want to define which skills are required for each department so the skill gap analysis (US-6.3) is meaningful.

---

## Acceptance Criteria

- [ ] `POST /api/admin/departments/{id}/required-skills` accepts `{ skillId }` and marks a skill as required for the department
- [ ] `DELETE /api/admin/departments/{id}/required-skills/{skillId}` removes a required skill from a department
- [ ] `GET /api/admin/departments/{id}/required-skills` returns the list of required skills for a department
- [ ] Frontend: on the department detail page, an "Edit Required Skills" section shows the current required skills with add/remove controls
- [ ] A department can have any number of required skills (0 to n)
- [ ] Required skills selector uses MUI `Autocomplete` populated from the active taxonomy

---

## Technical Notes

- `DepartmentRequiredSkill` join entity: `DepartmentId`, `SkillId`, `AddedAt`, `AddedByAdminId`
- This data drives the gap analysis query in US-6.3

---

## Unit Tests

**Backend (Department required skills command handler tests):**
- [ ] `AddRequiredSkillCommandHandler` links a skill to a department successfully
- [ ] `AddRequiredSkillCommandHandler` returns a conflict error if the skill is already required for the department
- [ ] `RemoveRequiredSkillCommandHandler` removes the link between a skill and a department
- [ ] `RemoveRequiredSkillCommandHandler` returns `NotFound` if the link does not exist
- [ ] `GetRequiredSkillsQueryHandler` returns all required skills for a given department

---

## Links

- [BACKLOG.md](../../BACKLOG.md)
- **Prev:** [US-8.4](US-8.4-deactivate-skill.md)

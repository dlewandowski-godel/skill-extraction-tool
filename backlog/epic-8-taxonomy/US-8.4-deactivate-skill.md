# US-8.4 — Deactivate Skill in Taxonomy

**Epic:** Epic 8 — Skill Taxonomy Management (Admin)  
**Status:** `[ ] Not Started` <!-- change to [x] Done when complete -->

---

## User Story

> As an admin, I want to deactivate a skill so it's no longer detected in future extractions without losing historical employee skill data.

---

## Acceptance Criteria

- [ ] `DELETE /api/admin/taxonomy/{id}` soft-deletes the skill by setting `IsActive = false`
- [ ] Deactivated skills are excluded from future extraction runs
- [ ] Historical `EmployeeSkill` records referencing the skill are retained (data integrity preserved)
- [ ] Deactivated skills are shown in the taxonomy list with an "Inactive" badge (visible but visually de-emphasized)
- [ ] An inactive skill can be re-activated via `PUT /api/admin/taxonomy/{id}/activate`
- [ ] Admin UI shows a "Deactivate" button with a confirmation dialog warning: "This skill will no longer be detected in future extractions. Existing employee data is preserved."
- [ ] Taxonomy cache is refreshed after deactivation

---

## Technical Notes

- Soft delete only — do NOT hard delete taxonomy entries to preserve referential integrity with `EmployeeSkill`
- Filter `IsActive = true` in the `MlNetSkillExtractor` taxonomy cache load

---

## Unit Tests

**Backend (`DeactivateSkillCommandHandlerTests`):**
- [ ] Sets `IsActive = false` on the target skill (soft delete)
- [ ] Returns `NotFound` for a non-existent skill ID
- [ ] Existing `EmployeeSkill` records referencing the deactivated skill are preserved
- [ ] Publishes `TaxonomyCacheInvalidatedEvent` to refresh the extractor cache

---

## Links

- [BACKLOG.md](../../BACKLOG.md)
- **Prev:** [US-8.3](US-8.3-edit-skill.md) | **Next:** [US-8.5](US-8.5-required-skills-per-dept.md)

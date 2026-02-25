# US-8.3 — Edit Skill in Taxonomy

**Epic:** Epic 8 — Skill Taxonomy Management (Admin)  
**Status:** `[ ] Not Started` <!-- change to [x] Done when complete -->

---

## User Story

> As an admin, I want to edit a skill's name, category, or aliases so the taxonomy stays accurate.

---

## Acceptance Criteria

- [ ] `PUT /api/admin/taxonomy/{id}` accepts `{ name, category, aliases: string[] }` and updates the skill
- [ ] Editing a skill name does not affect historical `EmployeeSkill` records (they reference by `SkillId`, not name)
- [ ] Aliases can be added or removed; at least one alias must remain
- [ ] Admin UI shows an "Edit" (pencil) icon on each skill row that opens an edit dialog pre-filled with current values
- [ ] Aliases are shown as MUI `Chip` components in the edit form with an 'x' to remove and a text field to add new ones
- [ ] On save, the taxonomy cache is refreshed (same mechanism as US-8.2)
- [ ] On success, the skills list updates in place

---

## Technical Notes

- For alias updates, use a replace strategy: delete all existing aliases for the skill, insert the new list
- Validate that the updated (name, category) combination doesn't conflict with another skill

---

## Unit Tests

**Backend (`UpdateSkillCommandHandlerTests`):**
- [ ] Successfully updates name, category, and aliases for an existing skill
- [ ] Returns `NotFound` for a non-existent skill ID
- [ ] Returns a conflict error when the updated (name, category) conflicts with another existing skill
- [ ] Uses replace strategy: removes all old aliases and inserts the new list
- [ ] Publishes `TaxonomyCacheInvalidatedEvent` after a successful update

---

## Links

- [BACKLOG.md](../../BACKLOG.md)
- **Prev:** [US-8.2](US-8.2-add-skill.md) | **Next:** [US-8.4](US-8.4-deactivate-skill.md)

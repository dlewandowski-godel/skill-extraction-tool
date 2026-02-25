# US-8.2 — Add Skill to Taxonomy

**Epic:** Epic 8 — Skill Taxonomy Management (Admin)  
**Status:** `[ ] Not Started` <!-- change to [x] Done when complete -->

---

## User Story

> As an admin, I want to add a new skill (name, category, aliases) to the taxonomy so the extraction engine can detect it in future document processing.

---

## Acceptance Criteria

- [ ] `POST /api/admin/taxonomy` accepts `{ name, category, aliases: string[] }` and creates the skill
- [ ] Skill name must be unique within a category; duplicates return `400 Bad Request`
- [ ] At least one alias is required (aliases are what the extractor matches against)
- [ ] The skill's name is automatically added to its own alias list if not already present
- [ ] New skill is created with `IsActive = true`
- [ ] Frontend "Add Skill" button opens a dialog with: name, category (autocomplete from existing categories), aliases (tag input)
- [ ] On success, the taxonomy cache in the extraction service is refreshed (cache invalidation event dispatched)
- [ ] On success, the taxonomy list is invalidated and the new skill appears in the correct category group

---

## Technical Notes

- Aliases are stored as a JSON array in a `text[]` PostgreSQL column, or a separate `SkillAlias` table (prefer separate table for indexability)
- Cache invalidation: publish a `TaxonomyCacheInvalidatedEvent` domain event → handled in `Infrastructure` to reload the extractor taxonomy cache

---

## Unit Tests

**Backend (`AddSkillCommandHandlerTests`):**
- [ ] Creates a skill with name, category, and aliases when all fields are valid
- [ ] Returns a conflict error when a skill with the same (name, category) already exists
- [ ] Publishes `TaxonomyCacheInvalidatedEvent` after a successful creation
- [ ] Persists all provided aliases in the `SkillAlias` table

---

## Links

- [BACKLOG.md](../../BACKLOG.md)
- **Prev:** [US-8.1](US-8.1-view-taxonomy.md) | **Next:** [US-8.3](US-8.3-edit-skill.md)

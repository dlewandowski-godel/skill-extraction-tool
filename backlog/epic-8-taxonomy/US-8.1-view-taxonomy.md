# US-8.1 — View Skill Taxonomy by Category

**Epic:** Epic 8 — Skill Taxonomy Management (Admin)  
**Status:** `[ ] Not Started` <!-- change to [x] Done when complete -->

---

## User Story

> As an admin, I want to view all skills in the taxonomy organized by category so I have a clear overview of the skill library that drives extraction.

---

## Acceptance Criteria

- [ ] `GET /api/admin/taxonomy` returns all skills grouped by category
- [ ] Each skill includes: `id`, `name`, `category`, `aliases[]`, `isActive`, `createdAt`
- [ ] `GET /api/admin/taxonomy?search=python&category=programming` supports filtering by name and category
- [ ] Frontend page at `/admin/taxonomy` shows an accordion or grouped list by category
- [ ] Each skill row shows: name, aliases count, status (active/inactive) badge
- [ ] Category counts are shown in the accordion headers (e.g., "Programming Languages (34)")
- [ ] Page has a search bar that filters skills in real-time (client-side filter on loaded data)

---

## Technical Notes

- Categories are free-text strings on the `Skill` entity (e.g., "Programming Languages", "Cloud Platforms", "Soft Skills")
- `GET /api/skills` (public to all authenticated users) is used by the admin skill selector in US-5.4

---

## Unit Tests

**Backend (`GetTaxonomyQueryHandlerTests`):**
- [ ] Returns all active skills grouped by category
- [ ] Excludes skills where `IsActive = false`
- [ ] Returns empty list when no skills exist
- [ ] Each skill includes its aliases

---

## Links

- [BACKLOG.md](../../BACKLOG.md)
- **Next:** [US-8.2](US-8.2-add-skill.md)

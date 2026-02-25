# US-5.2 — Proficiency Indicators on Skills

**Epic:** Epic 5 — Employee Skill Profile  
**Status:** `[ ] Not Started` <!-- change to [x] Done when complete -->

---

## User Story

> As a user, I want to see a proficiency indicator (badge or chip) next to each skill so I know what level was inferred for me.

---

## Acceptance Criteria

- [ ] Each skill displays its proficiency level as a MUI `Chip` with color coding:
  - `Beginner` → grey
  - `Intermediate` → blue
  - `Advanced` → orange
  - `Expert` → green
- [ ] Skills with `IsManualOverride = true` show a small "edited" icon (pencil) on their chip as a tooltip indicating an admin set this level
- [ ] Chips are rendered inline next to skill names
- [ ] Proficiency chips are also shown in admin views (US-5.3, US-5.6)
- [ ] A `<ProficiencyChip level={...} isOverride={...} />` component is created for reuse across all skill display contexts

---

## Technical Notes

- Create a `ProficiencyChip` component in `src/components/skills/ProficiencyChip.tsx`
- Use MUI `Chip` with `color` and `size="small"` props
- Component should have a Vitest unit test (see US-10.4)

---

## Unit Tests

**Frontend (`ProficiencyChip.test.tsx`):**
- [ ] Renders "Beginner" label with grey color when `level="Beginner"`
- [ ] Renders "Intermediate" label with blue color when `level="Intermediate"`
- [ ] Renders "Advanced" label with orange color when `level="Advanced"`
- [ ] Renders "Expert" label with green color when `level="Expert"`
- [ ] Shows a pencil/edit icon and "Manually set" tooltip when `isOverride={true}`
- [ ] Does not render an edit icon when `isOverride={false}`

---

## Links

- [BACKLOG.md](../../BACKLOG.md)
- **Prev:** [US-5.1](US-5.1-view-own-profile.md) | **Next:** [US-5.3](US-5.3-admin-view-profile.md)

# US-4.5 — Persist Extracted Skills

**Epic:** Epic 4 — Skill Extraction Engine  
**Status:** `[x] Done` <!-- change to [x] Done when complete -->

---

## User Story

> As the system, I want extracted and mapped skills persisted in the database linked to the employee's profile so results are retrievable at any time.

---

## Acceptance Criteria

- [ ] An `EmployeeSkill` entity exists in `Domain` with: `Id`, `UserId`, `SkillId`, `ProficiencyLevel`, `IsManualOverride`, `SourceDocumentId`, `ExtractedAt`
- [ ] When a document is processed, existing auto-extracted skills from that document type (CV or IFU) are replaced (not duplicated)
- [ ] Manual overrides (`IsManualOverride = true`) are never overwritten by re-extraction
- [ ] Skills from CV and IFU are merged in the profile: if both docs extract the same skill, the higher proficiency level wins
- [ ] `IEmployeeSkillRepository` interface is defined in `Domain`; implementation in `Infrastructure`
- [ ] Persistence uses EF Core bulk insert for performance (all skills in one `SaveChangesAsync()` call)
- [ ] Database schema includes a unique index on `(UserId, SkillId)` to prevent duplicate skill entries

---

## Technical Notes

- Use `EF Core ExecuteDeleteAsync` to remove old auto-extracted skills before inserting new ones
- `EmployeeSkill` uses a composite key or a surrogate GUID key; prefer surrogate key for EF simplicity

---

## Unit Tests

**Backend (`EmployeeSkillEntityTests` — Domain):**
- [ ] Cannot create `EmployeeSkill` without a valid `UserId` and `SkillId`
- [ ] Calling the proficiency-change method sets `IsManualOverride = true`
- [ ] Manual overrides cannot be overwritten by a subsequent auto-extraction (domain invariant enforced)

**Backend (`PersistExtractedSkillsCommandHandlerTests`):**
- [ ] Existing auto-extracted skills from the same document type are removed before inserting new ones
- [ ] Skills with `IsManualOverride = true` are NOT removed by re-extraction
- [ ] When CV and IFU both extract the same skill, higher proficiency level is persisted
- [ ] All new skills are saved in a single `SaveChangesAsync()` call

---

## Links

- [BACKLOG.md](../../BACKLOG.md)
- **Prev:** [US-4.4](US-4.4-process-document-command.md) | **Next:** [US-4.6](US-4.6-extraction-unit-tests.md)

# US-4.3 — Proficiency Level Inference

**Epic:** Epic 4 — Skill Extraction Engine  
**Status:** `[x] Done` <!-- change to [x] Done when complete -->

---

## User Story

> As the system, I want to infer a proficiency level (Beginner / Intermediate / Advanced / Expert) from contextual keywords near each matched skill so the skill profile contains meaningful depth.

---

## Acceptance Criteria

- [ ] Proficiency levels are: `Beginner`, `Intermediate`, `Advanced`, `Expert`
- [ ] A configurable keyword map is used for inference:

  | Level | Keywords |
  |-------|----------|
  | Beginner | "familiar with", "basic", "introductory", "learning", "exposure to" |
  | Intermediate | "working knowledge", "experience with", "used", "implemented" |
  | Advanced | "advanced", "deep knowledge", "led", "designed", "architected" |
  | Expert | "expert", "10+ years", "specialist", "authored", "speaker" |

- [ ] The algorithm checks a window of ±10 words around each matched skill token
- [ ] If no proficiency keyword is found near a skill, default level is `Intermediate`
- [ ] If multiple proficiency levels are inferred for the same skill (from multiple occurrences), the highest level wins
- [ ] Admin can override the inferred level on any skill (see US-5.6) — manual overrides are not re-inferred
- [ ] Proficiency keyword map is stored in a config file / database table so it can be updated without redeployment

---

## Technical Notes

- Context window scanning operates on the raw token list, not the full text string (faster and more accurate)
- `ExtractedSkill` DTO includes `ProficiencyLevel` and `IsManualOverride` flag

---

## Unit Tests

**Backend (`ProficiencyInferenceServiceTests`):**
- [ ] Text with "familiar with Python" near Python infers `Beginner`
- [ ] Text with "experience with Python" near Python infers `Intermediate`
- [ ] Text with "advanced Python" near Python infers `Advanced`
- [ ] Text with "expert in Python" near Python infers `Expert`
- [ ] Text with no proficiency keyword within ±10 words of the skill defaults to `Intermediate`
- [ ] When the same skill has both "familiar with" and "expert in" in different sentences, `Expert` wins
- [ ] Context window boundary is respected: keyword > 10 tokens away from skill is not considered

---

## Links

- [BACKLOG.md](../../BACKLOG.md)
- **Prev:** [US-4.2](US-4.2-mlnet-matching.md) | **Next:** [US-4.4](US-4.4-process-document-command.md)

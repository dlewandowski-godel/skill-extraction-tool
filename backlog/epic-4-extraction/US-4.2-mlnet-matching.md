# US-4.2 — ML.NET Tokenization + Taxonomy Matching

**Epic:** Epic 4 — Skill Extraction Engine  
**Status:** `[x] Done` <!-- change to [x] Done when complete -->

---

## User Story

> As the system, I want to tokenize extracted text using ML.NET and match tokens/phrases against the predefined skill taxonomy so skills mentioned in documents are identified.

---

## Acceptance Criteria

- [ ] `Microsoft.ML` NuGet package is installed in `Infrastructure`
- [ ] `ISkillExtractor` interface is defined in `Application`: `IReadOnlyList<ExtractedSkill> ExtractSkills(string text)`
- [ ] `MlNetSkillExtractor` implementation in `Infrastructure` performs:
  1. Tokenize text into words using `mlContext.Transforms.Text.TokenizeIntoWords`
  2. Normalize to lowercase
  3. Match single tokens and bi-grams against skill aliases from the taxonomy
- [ ] Matching is case-insensitive and supports aliases (e.g., "C#" and "CSharp" both map to the same skill)
- [ ] The taxonomy is loaded from the database at startup and cached in-memory
- [ ] Returns a list of `ExtractedSkill` with: `SkillId`, `SkillName`, `Category`, `MatchedAlias`, `OccurrenceCount`
- [ ] Skills are deduplicated (same skill matched multiple times → `OccurrenceCount` increases)
- [ ] Taxonomy cache is refreshed when a skill is added/updated via admin (cache invalidation event)

---

## Technical Notes

- For bi-gram matching, use `ProduceNgrams` (n=2) on the tokenized output
- Taxonomy aliases stored as a `IReadOnlyDictionary<string, SkillTaxonomyEntry>` for O(1) lookup
- `MlNetSkillExtractor` is registered as a singleton (ML context is expensive to create)

---

## Unit Tests

**Backend (`MlNetSkillExtractorTests`):**
- [ ] Text containing an exact skill name returns exactly that skill
- [ ] Text with multiple different skills returns all of them without duplicates
- [ ] Text with no known skills returns an empty list
- [ ] Alias matching works: "CSharp" and "C#" both resolve to the same skill entry
- [ ] Matching is case-insensitive ("python" and "Python" produce the same result)
- [ ] Repeated mentions of the same skill increment `OccurrenceCount`, not create duplicates
- [ ] Bi-gram phrase skills (e.g., "machine learning") are matched as a single skill

---

## Links

- [BACKLOG.md](../../BACKLOG.md)
- **Prev:** [US-4.1](US-4.1-pdf-text-extraction.md) | **Next:** [US-4.3](US-4.3-proficiency-inference.md)
